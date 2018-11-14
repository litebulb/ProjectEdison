using Automatonymous;
using Edison.Common.Config;
using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Edison.Workflows.Config;
using MassTransit;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Edison.Workflows
{
    internal class ResponseStateMachine : MassTransitStateMachine<ResponseState>
    {
        //Warning keep concurrency limit to 1
        private readonly ServiceBusRabbitMQOptions _configBus;
        //private readonly WorkflowConfigResponse _configWorkflow;
        private int _closeEventsCount = 0;
        private int _closeEventsComplete = 0;

        public ResponseStateMachine(IOptions<ServiceBusRabbitMQOptions> configBus, IOptions<WorkflowConfig> configWorkflow)
        {
            _configBus = configBus.Value;
            //_configWorkflow = configWorkflow.Value.ResponseWorkflow;

            InstanceState(x => x.State);

            #region Initialization Events
            //Devices Updates
            Event(() => NewResponseCreated, x => x.CorrelateById(context => context.Message.ResponseModel.ResponseId));
            //UI Update notification, can be ignored if saga doesn't exist anymore
            Event(() => ResponseActionsUpdated, x => x.CorrelateById(context => context.Message.ResponseId));
            Event(() => ResponseActionClosed, x => x.CorrelateById(context => context.Message.ResponseId));
            Event(() => ResponseClosed, x => x.CorrelateById(context => context.Message.ResponseModel.ResponseId));
            Event(() => ResponseEventClusterTagged, x => x.CorrelateById(context => context.Message.Response.ResponseId));
            Event(() => ResponseFinalize, x => x.CorrelateById(context => context.Message.ResponseId));

            #endregion

            //The event is submitted from Azure Service Queue
            Initially(
                When(NewResponseCreated)
                    .Then(context => context.Instance.CorrelationId = context.Data.ResponseModel.ResponseId)
                    .ThenAsync(context =>
                    Console.Out.WriteLineAsync($"Response-{context.Instance.CorrelationId}: Saga Initiated."))
                    .ThenAsync(context => Console.Out.WriteLineAsync($"Response--{context.Instance.CorrelationId}: Event Received."))
                    .ThenAsync(context => context.Publish(new ResponseTagExistingEventClustersRequestedEvent()
                    {
                        ResponseId = context.Data.ResponseModel.ResponseId,
                        ResponseGeolocation = context.Data.ResponseModel.Geolocation,
                        Radius = context.Data.ResponseModel.ActionPlan.PrimaryRadius
                    }))
                    .ThenAsync(context => context.Publish(new ResponseUIUpdateRequestedEvent()
                    {
                        ResponseId = context.Instance.CorrelationId,
                        ResponseUI = new ResponseUIModel()
                        {
                            UpdateType = "NewResponse",
                            Response = context.Data.ResponseModel,
                            ResponseId = context.Data.ResponseModel.ResponseId
                        }
                    }))
                    //Run open actions
                     .ThenAsync(context => context.Data.ResponseModel.ActionPlan.OpenActions.TaskForEach(action => context.Publish(
                        new ActionEvent()
                        {
                            ResponseId = context.Data.ResponseModel.ResponseId,
                            Action = action,
                            Geolocation = context.Data.ResponseModel.Geolocation,
                            PrimaryRadius = context.Data.ResponseModel.ActionPlan.PrimaryRadius,
                            SecondaryRadius = context.Data.ResponseModel.ActionPlan.SecondaryRadius
                        })))
                    .ThenAsync(context => Console.Out.WriteLineAsync($"ResponseModel--{context.Instance.CorrelationId}: NewResponseCreated."))
                    .TransitionTo(Waiting)
                    );

            During(Waiting,
                When(ResponseEventClusterTagged)
                .ThenAsync(context => context.Publish(new ResponseUIUpdateRequestedEvent()
                {
                    ResponseId = context.Instance.CorrelationId,
                    ResponseUI = new ResponseUIModel()
                    {
                        UpdateType = "UpdateResponse",
                        Response = context.Data.Response,
                        ResponseId = context.Data.Response.ResponseId
                    }
                })),
                When(ResponseActionsUpdated)
                .Then(context => Console.Out.WriteLineAsync($"ResponseModel--{context.Instance.CorrelationId}: ResponseActionsUpdated."))
                //Run close actions
                .ThenAsync(context => context.Data.Actions.TaskForEach(action =>
                        context.Publish(
                            new ActionEvent()
                            {
                                ResponseId = context.Data.ResponseId,
                                Action = action,
                                Geolocation = context.Data.Geolocation,
                                PrimaryRadius = context.Data.PrimaryRadius,
                                SecondaryRadius = context.Data.SecondaryRadius
                            })))
                 .ThenAsync(context => context.Publish(new ResponseUIUpdateRequestedEvent()
                 {
                     ResponseId = context.Instance.CorrelationId,
                     ResponseUI = new ResponseUIModel()
                     {
                         UpdateType = "UpdateResponseActions",
                         Response = new ResponseModel()
                         {
                             ResponseId = context.Data.ResponseId,
                             ActionPlan = new ResponseActionPlanModel() {
                                 OpenActions = context.Data.Actions }
                         },
                         ResponseId = context.Data.ResponseId
                     }
                 })),
                When(ResponseActionClosed)
                .Then(context => Console.Out.WriteLineAsync($"ResponseModel--{context.Instance.CorrelationId}: ResponseActionClosed"))
                 //Track action callbacks
                 .ThenAsync(context => context.Publish(new ActionCloseUIUpdatedRequestedEvent()
                 {
                     ResponseId = context.Data.ResponseId,
                     ActionId = context.Data.ActionId,
                     ActionCloseModel = new ActionCloseUIModel()
                     {
                         UpdateType = "CloseResponseAction",
                         ActionId = context.Data.ActionId,
                         IsSuccessful = context.Data.IsSuccessful,
                         ResponseId = context.Data.ResponseId
                     }
                 }))
                 .If(new StateMachineCondition<ResponseState, IEventSagaReceiveResponseActionClosed>(bc =>
                    bc.Data.IsCloseAction), x => x
                    .ThenAsync(context =>
                    {
                        _closeEventsComplete += 1;
                        return context.Publish(new EventSagaFinalize() { ResponseId = context.Data.ResponseId });
                    })),
                When(ResponseClosed)
                .Then(context => Console.Out.WriteLineAsync($"ResponseModel--{context.Instance.CorrelationId}: ResponseClosed."))
                //Run close actions
                .ThenAsync(context =>
                {
                    _closeEventsCount = context.Data.ResponseModel.ActionPlan.CloseActions.Count();
                    return context.Data.ResponseModel.ActionPlan.CloseActions.TaskForEach(action => context.Publish(
                            new ActionEvent(true)
                            {
                                ResponseId = context.Data.ResponseModel.ResponseId,
                                Action = action,
                                Geolocation = context.Data.ResponseModel.Geolocation,
                                PrimaryRadius = context.Data.ResponseModel.ActionPlan.PrimaryRadius,
                                SecondaryRadius = context.Data.ResponseModel.ActionPlan.SecondaryRadius
                            }));
                })
                 .ThenAsync(context => context.Publish(new ResponseUIUpdateRequestedEvent()
                 {
                     ResponseId = context.Instance.CorrelationId,
                     ResponseUI = new ResponseUIModel()
                     {
                         UpdateType = "CloseResponse",
                         Response = context.Data.ResponseModel,
                         ResponseId = context.Data.ResponseModel.ResponseId
                     }
                 })),
                When(ResponseFinalize)
                .If(new StateMachineCondition<ResponseState, IEventSagaFinalize>(bc =>
                    _closeEventsComplete >= _closeEventsCount), x => x.Finalize()));

            //Delete persisted saga after completion
            SetCompletedWhenFinalized();
        }

        public State Waiting { get; private set; }


        #region "Events"
        public Event<IResponseTaggedEventClusters> ResponseEventClusterTagged { get; private set; }
        public Event<IEventSagaReceiveResponseCreated> NewResponseCreated { get; private set; }
        public Event<IEventSagaReceiveResponseActionsUpdated> ResponseActionsUpdated { get; private set; }
        public Event<IEventSagaReceiveResponseActionClosed> ResponseActionClosed { get; private set; }
        public Event<IEventSagaReceiveResponseClosed> ResponseClosed { get; private set; }
        public Event<IEventSagaFinalize> ResponseFinalize { get; private set; }
        #endregion
    }
}
