using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit;
using Automatonymous;
using Automatonymous.Binders;
using Edison.Core.Common;
using Edison.Common.Messages;
using Edison.Core.Common.Models;
using Edison.Common.Messages.Interfaces;

namespace Edison.Workflows.Extensions
{
    public static class ResponseSagaExtensions
    {
        internal static EventActivityBinder<ResponseState, T> ThenPublishActions<T>(this EventActivityBinder<ResponseState, T> binder, ResponseUpdateType responseUpdateType) where T : class, IResponseMessage
        {
            return binder.ThenAsync(async context =>
            {
                //Retrieve the list of actions
                IEnumerable<ResponseActionModel> actions = responseUpdateType == ResponseUpdateType.CloseResponse
                ? context.Data.Response.ActionPlan.CloseActions : context.Data.Response.ActionPlan.OpenActions;
                var actionStatus = context.Data.ActionStatus;
                actions = actions.Where(p => 
                    (actionStatus.HasFlag(ActionStatus.Error) && p.Status == ActionStatus.Error) ||
                    (actionStatus.HasFlag(ActionStatus.NotStarted) && p.Status == ActionStatus.NotStarted) ||
                    (actionStatus.HasFlag(ActionStatus.Skipped) && p.Status == ActionStatus.Skipped) ||
                    (actionStatus.HasFlag(ActionStatus.Success) && p.Status == ActionStatus.Success)
                );

                //If the response is not of type CloseResponse, we trigger the event cluster associated
                if (responseUpdateType != ResponseUpdateType.CloseResponse)
                {
                    await context.Publish(new ResponseTagExistingEventClustersRequestedEvent()
                    {
                        ResponseId = context.Data.Response.ResponseId,
                        ResponseGeolocation = context.Data.Response.Geolocation,
                        Radius = context.Data.Response.ActionPlan.PrimaryRadius
                    });
                }

                //If there is no action to update, we send the UI update immediately.
                if(actions == null || actions.Count() == 0)
                {
                    await context.Publish(new ResponseUIUpdateRequestedEvent()
                    {
                        ResponseId = context.Instance.CorrelationId,
                        ResponseUI = new ResponseUIModel()
                        {
                            UpdateType = context.Instance.ActionUpdateType.ToString(),
                            ResponseId = context.Instance.CorrelationId
                        }
                    });
                    return;
                }

                context.Instance.ActionCorrelationId = Guid.NewGuid();
                context.Instance.ActionUpdateType = responseUpdateType;
                context.Instance.ActionsTotal = actions.Count();
                context.Instance.ActionsCompletedCount = 0;
                await actions.TaskForEach(async action => {
                    await Console.Out.WriteLineAsync($"Response--{context.Instance.CorrelationId}: Start Action {action.ActionId} of type {action.ActionType}.");
                    await context.Publish(new ActionEvent()
                    {
                        ActionCorrelationId = context.Instance.ActionCorrelationId,
                        ActionId = action.ActionId,
                        ResponseId = context.Data.Response.ResponseId,
                        Action = action,
                        Geolocation = context.Data.Response.Geolocation,
                        PrimaryRadius = context.Data.Response.ActionPlan.PrimaryRadius,
                        SecondaryRadius = context.Data.Response.ActionPlan.SecondaryRadius                        
                    });
                });
            });
        }
    }
}
