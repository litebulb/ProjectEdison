using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Automatonymous;
using MassTransit;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Edison.Common.Config;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;
using Edison.Workflows.Config;

namespace Edison.Workflows
{
    /// <summary>
    /// Saga to handle the lifecycle of a response
    /// </summary>
    internal class ResponseStateMachine : MassTransitStateMachine<ResponseState>
    {
        //Warning keep concurrency limit to 1
        private readonly ServiceBusRabbitMQOptions _configBus;

        public ResponseStateMachine(IOptions<ServiceBusRabbitMQOptions> configBus, IOptions<WorkflowConfig> configWorkflow)
        {
            _configBus = configBus.Value;

            InstanceState(x => x.State);

            //Response events
            Event(() => NewResponseCreated, x => x.CorrelateById(context => context.Message.ResponseModel.ResponseId));
            //UI Update notification, can be ignored if saga doesn't exist anymore
            Event(() => ResponseActionsUpdated, x => x.CorrelateById(context => context.Message.ResponseId));
            Event(() => ResponseActionCallback, x => x.CorrelateById(context => context.Message.ResponseId));
            Event(() => ResponseClosed, x => x.CorrelateById(context => context.Message.ResponseModel.ResponseId));
            Event(() => ResponseEventClusterTagged, x => x.CorrelateById(context => context.Message.Response.ResponseId));

            //The event is submitted from the Response endpoint
            Initially(
                When(NewResponseCreated)
                    .Then(context => context.Instance.CorrelationId = context.Data.ResponseModel.ResponseId)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"Response-{context.Instance.CorrelationId}: Saga Initiated."))
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
                     .ThenAsync(context => context.Data.ResponseModel.ActionPlan.OpenActions
                     .TaskForEach(action => context.Publish(
                        new ActionEvent()
                        {
                            ActionId = action.ActionId,
                            ResponseId = context.Data.ResponseModel.ResponseId,
                            Action = action,
                            Geolocation = context.Data.ResponseModel.Geolocation,
                            PrimaryRadius = context.Data.ResponseModel.ActionPlan.PrimaryRadius,
                            SecondaryRadius = context.Data.ResponseModel.ActionPlan.SecondaryRadius
                        })))
                    .ThenAsync(context => Console.Out.WriteLineAsync($"Response--{context.Instance.CorrelationId}: NewResponseCreated."))
                    .TransitionTo(Waiting)
            );

            //Waiting state
            During(Waiting,
                //Triggers when event clusters have been associated
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

                //Triggers when the response was closed
                When(ResponseClosed)
                .Then(context => Console.Out.WriteLineAsync($"Response--{context.Instance.CorrelationId}: ResponseClosed."))
                .ThenAsync(context => //Run close actions
                {
                    context.Instance.CloseActionsCount = 0;
                    context.Instance.CloseActionsTotal = context.Data.ResponseModel.ActionPlan.CloseActions.Count();
                    context.Instance.ActionPlanCloseTriggered = true;
                    return context.Data.ResponseModel.ActionPlan.CloseActions.TaskForEach(action => context.Publish(
                            new ActionEvent()
                            {
                                IsCloseAction = true,
                                ActionId = action.ActionId,
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

                //Triggers when the was updated with new actions
                When(ResponseActionsUpdated)
                .Then(context => Console.Out.WriteLineAsync($"Response--{context.Instance.CorrelationId}: ResponseActionsUpdated."))
                .ThenAsync(context => context.Publish(new ResponseTagExistingEventClustersRequestedEvent()
                {
                    ResponseId = context.Data.ResponseId,
                    ResponseGeolocation = context.Data.Geolocation,
                    Radius = context.Data.PrimaryRadius
                }))
                .ThenAsync(context => context.Data.Actions.Where(p => p.Status == ActionStatus.NotRun || p.Status == ActionStatus.Skipped).TaskForEach(action =>
                        context.Publish(new ActionEvent()
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
                            ActionPlan = new ResponseActionPlanModel()
                            {
                                OpenActions = context.Data.Actions
                            }
                        },
                        ResponseId = context.Data.ResponseId
                    }
                })),

                //Triggers call back on actions, ends the saga if all close actions are performed successfully
                When(ResponseActionCallback)
                .Then(context => Console.Out.WriteLineAsync($"Response--{context.Instance.CorrelationId}: ResponseActionCallback"))
                .ThenAsync(context => context.Publish(new ActionCallbackUIUpdatedRequestedEvent() //Track action callbacks
                {
                     ResponseId = context.Data.ResponseId,
                     ActionId = context.Data.ActionId,
                     ActionCloseModel = new ActionCallbackUIModel()
                     {
                         UpdateType = "ResponseActionCallback",
                         ActionId = context.Data.ActionId,
                         Status = context.Data.Status,
                         ErrorMessage = context.Data.ErrorMessage,
                         StartDate = context.Data.StartDate,
                         EndDate = context.Data.EndDate,
                         ResponseId = context.Data.ResponseId
                     }
                }))
                .If(new StateMachineCondition<ResponseState, IEventSagaReceiveResponseActionCallback>(bc =>
                    bc.Data.IsCloseAction && (bc.Data.Status == ActionStatus.Success || bc.Data.Status == ActionStatus.Skipped)), x => x
                    .Then(context =>
                    {
                        context.Instance.CloseActionsCount++;
                        if (context.Instance.ActionPlanCloseTriggered && context.Instance.CloseActionsCount >= context.Instance.CloseActionsTotal)
                            x.Finalize();
                    }))
                );

            //Delete persisted saga after completion
            SetCompletedWhenFinalized();
        }

        public State Waiting { get; private set; }


        #region "Events"
        public Event<IResponseTaggedEventClusters> ResponseEventClusterTagged { get; private set; }
        public Event<IEventSagaReceiveResponseCreated> NewResponseCreated { get; private set; }
        public Event<IEventSagaReceiveResponseActionsUpdated> ResponseActionsUpdated { get; private set; }
        public Event<IEventSagaReceiveResponseActionCallback> ResponseActionCallback { get; private set; }
        public Event<IEventSagaReceiveResponseClosed> ResponseClosed { get; private set; }
        #endregion
    }
}
