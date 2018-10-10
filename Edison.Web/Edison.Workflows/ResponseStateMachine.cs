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
using System.Threading.Tasks;

namespace Edison.Workflows
{
    internal class ResponseStateMachine : MassTransitStateMachine<ResponseState>
    {
        //Warning keep concurrency limit to 1
        private readonly ServiceBusOptions _configBus;
        //private readonly WorkflowConfigResponse _configWorkflow;

        public ResponseStateMachine(IOptions<ServiceBusOptions> configBus, IOptions<WorkflowConfig> configWorkflow)
        {
            _configBus = configBus.Value;
            //_configWorkflow = configWorkflow.Value.ResponseWorkflow;

            InstanceState(x => x.State);

            #region Initialization Events
            //Devices Updates
            Event(() => NewResponseCreated, x => x.CorrelateById(context => context.Message.ResponseModel.ResponseId));
            //UI Update notification, can be ignored if saga doesn't exist anymore
            Event(() => ResponseUpdated, x => x.CorrelateById(context => context.Message.ResponseModel.ResponseId));
            Event(() => ResponseEventClusterTagged, x => x.CorrelateById(context => context.Message.Response.ResponseId));

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
                When(ResponseUpdated)
                .Then(context => Console.Out.WriteLineAsync($"ResponseModel--{context.Instance.CorrelationId}: ResponseUpdated."))
                //Run close actions
                .ThenAsync(context => context.Data.ResponseModel.ActionPlan.CloseActions.TaskForEach(action => context.Publish(
                        new ActionEvent()
                        {
                            Action = action,
                            Geolocation = context.Data.ResponseModel.Geolocation,
                            PrimaryRadius = context.Data.ResponseModel.ActionPlan.PrimaryRadius,
                            SecondaryRadius = context.Data.ResponseModel.ActionPlan.SecondaryRadius
                        })))
                 .ThenAsync(context => context.Publish(new ResponseUIUpdateRequestedEvent()
                 {
                     ResponseId = context.Instance.CorrelationId,
                     ResponseUI = new ResponseUIModel()
                     {
                         UpdateType = "CloseResponse",
                         Response = context.Data.ResponseModel,
                         ResponseId = context.Data.ResponseModel.ResponseId
                     }
                 })).Finalize());

            //Delete persisted saga after completion
            SetCompletedWhenFinalized();
        }

        public State Waiting { get; private set; }


        #region "Events"
        public Event<IResponseTaggedEventClusters> ResponseEventClusterTagged { get; private set; }
        public Event<IEventSagaReceiveResponseCreated> NewResponseCreated { get; private set; }
        public Event<IEventSagaReceiveResponseUpdated> ResponseUpdated { get; private set; }
        #endregion
    }
}
