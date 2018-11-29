using System;
using Microsoft.Extensions.Options;
using MassTransit;
using Automatonymous;
using Edison.Core.Common.Models;
using Edison.Common.Config;
using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Workflows.Config;
using Edison.Workflows.Extensions;

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
            Event(() => ResponseCreated, x => x.CorrelateById(context => context.Message.Response.ResponseId));
            Event(() => ResponseUpdated, x => x.CorrelateById(context => context.Message.Response.ResponseId));
            Event(() => ResponseActionCallback, x => x.CorrelateById(context => context.Message.ResponseId));
            Event(() => ResponseClosed, x => x.CorrelateById(context => context.Message.Response.ResponseId));
            Event(() => ResponseEventClusterTagged, x => x.CorrelateById(context => context.Message.Response.ResponseId));

            //The event is submitted from the Response endpoint
            Initially(
                When(ResponseCreated)
                    .Then(context => context.Instance.CorrelationId = context.Data.Response.ResponseId)
                    .Then(context => Console.WriteLine($"Response-{context.Instance.CorrelationId}: Saga Initiated."))
                    .Then(context => Console.WriteLine($"Response--{context.Instance.CorrelationId}: Event Received."))
                    .ThenPublishActions(ResponseUpdateType.NewResponse) //Run open actions
                    .Then(context => Console.Out.WriteLine($"Response--{context.Instance.CorrelationId}: ResponseCreated."))
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
                        UpdateType = ResponseUpdateType.UpdateResponse.ToString(),
                        Response = context.Data.Response,
                        ResponseId = context.Data.Response.ResponseId
                    }
                })),

                //Triggers when the response was closed
                When(ResponseClosed)
                .Then(context => Console.Out.WriteLine($"Response--{context.Instance.CorrelationId}: ResponseClosed."))
                .ThenPublishActions(ResponseUpdateType.CloseResponse), //Run close actions

                //Triggers when the was updated with new actions
                When(ResponseUpdated)
                .Then(context => Console.Out.WriteLine($"Response--{context.Instance.CorrelationId}: ResponseActionsUpdated."))
                .ThenPublishActions(ResponseUpdateType.UpdateResponseActions, p => p.Status == ActionStatus.NotRun || p.Status == ActionStatus.Skipped), //Run update action,

                //Triggers call back on actions, ends the saga if all close actions are performed successfully
                When(ResponseActionCallback)
                .Then(context => Console.Out.WriteLine($"Response--{context.Instance.CorrelationId}: ResponseActionCallback"))
                .ThenAsync(context => context.Publish(new ActionCallbackUIUpdatedRequestedEvent() //Track action callbacks
                {
                    ResponseId = context.Data.ResponseId,
                    ActionId = context.Data.ActionId,
                    ActionCloseModel = new ActionCallbackUIModel()
                    {
                        UpdateType = ResponseUpdateType.ResponseActionCallback.ToString(),
                        ActionId = context.Data.ActionId,
                        Status = context.Data.Status,
                        ErrorMessage = context.Data.ErrorMessage,
                        StartDate = context.Data.StartDate,
                        EndDate = context.Data.EndDate,
                        ResponseId = context.Data.ResponseId
                    }
                }))
                //Add 1 to the action completed count
                .If(new StateMachineCondition<ResponseState, IEventSagaReceiveResponseActionCallback>(bc =>
                    bc.Data.ActionCorrelationId == bc.Instance.ActionCorrelationId), x => x
                    .Then(context => context.Instance.ActionsCompletedCount++))
                //When the count is complete, we send a UI update
                .If(new StateMachineCondition<ResponseState, IEventSagaReceiveResponseActionCallback>(bc =>
                    bc.Data.ActionCorrelationId == bc.Instance.ActionCorrelationId && bc.Instance.ActionsCompletedCount >= bc.Instance.ActionsTotal), x => x
                    .ThenAsync(context => context.Publish(new ResponseUIUpdateRequestedEvent()
                    {
                        ResponseId = context.Instance.CorrelationId,
                        ResponseUI = new ResponseUIModel()
                        {
                            UpdateType = context.Instance.ActionUpdateType.ToString(),
                            ResponseId = context.Instance.CorrelationId
                        }
                    })))
                //If the update type was close response, we finalize the saga
                .If(new StateMachineCondition<ResponseState, IEventSagaReceiveResponseActionCallback>(bc =>
                    bc.Data.ActionCorrelationId == bc.Instance.ActionCorrelationId && bc.Instance.ActionsCompletedCount >= bc.Instance.ActionsTotal 
                    && bc.Instance.ActionUpdateType == ResponseUpdateType.CloseResponse), x => x
                    .Finalize())
                );

            //Delete persisted saga after completion
            SetCompletedWhenFinalized();
        }

        public State Waiting { get; private set; }

        #region "Events"
        public Event<IResponseTaggedEventClusters> ResponseEventClusterTagged { get; private set; }
        public Event<IEventSagaReceiveResponseCreated> ResponseCreated { get; private set; }
        public Event<IEventSagaReceiveResponseUpdated> ResponseUpdated { get; private set; }
        public Event<IEventSagaReceiveResponseActionCallback> ResponseActionCallback { get; private set; }
        public Event<IEventSagaReceiveResponseClosed> ResponseClosed { get; private set; }
        #endregion
    }
}
