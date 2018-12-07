using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MassTransit;
using Edison.Core.Interfaces;
using Edison.Core.Common.Models;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;

namespace Edison.ResponseService.Consumers
{
    /// <summary>
    /// Masstransit consumer that handles a generic action from a response
    /// </summary>
    public class ResponseActionEventConsumer : ResponseActionBaseConsumer, IConsumer<IActionEvent>
    {
        public ResponseActionEventConsumer(IResponseRestService responseRestService,
            ILogger<ResponseActionEventConsumer> logger) : base (responseRestService, logger)
        {
        }

        public async Task Consume(ConsumeContext<IActionEvent> context)
        {
            try
            {
                _logger.LogDebug($"ResponseActionEventConsumer: Retrieved message from response '{context.Message.Action.ActionId}'.");
                ResponseActionModel action = context.Message.Action;
                if (!string.IsNullOrEmpty(action.ActionType))
                {
                    _logger.LogDebug($"ResponseActionEventConsumer: ActionType retrieved: '{action.ActionType}'.");
                    //Switch statement directing different action types
                    switch(action.ActionType)
                    {
                        case "notification":
                            _logger.LogDebug($"ResponseActionEventConsumer: Publish ActionNotificationEvent.");
                            await context.Publish(new ActionNotificationEvent(action) {
                                ResponseId = context.Message.ResponseId,
                                ActionCorrelationId = context.Message.ActionCorrelationId
                            });
                            break;
                        case "lightsensor":
                            _logger.LogDebug($"ResponseActionEventConsumer: Publish ActionLightSensorEvent.");
                            await context.Publish(new ActionLightSensorEvent(action) {
                                ActionCorrelationId = context.Message.ActionCorrelationId,
                                ResponseId = context.Message.ResponseId,
                                GeolocationPoint = context.Message.Geolocation,
                                PrimaryRadius = context.Message.PrimaryRadius,
                                SecondaryRadius = context.Message.SecondaryRadius
                            });
                            break;
                        case "twilio":
                            _logger.LogDebug($"ResponseActionEventConsumer: Publish ActionTwilioEvent.");
                            await context.Publish(new ActionTwilioEvent(action)
                            {
                                ResponseId = context.Message.ResponseId,
                                Message = context.Message.Action.Parameters["message"],
                                ActionCorrelationId = context.Message.ActionCorrelationId
                            });
                            break;
                        default:
                            await GenerateActionCallback(context, ActionStatus.Skipped, DateTime.UtcNow, $"The action type '{action.ActionType}' cannot be handled.");
                            break;
                    }
                    return;
                }
                _logger.LogError("ResponseActionEventConsumer: Invalid Null or Empty Action Type");
                throw new Exception("Invalid Null or Empty Action Type");

            }
            catch (Exception e)
            {
                await GenerateActionCallback(context, ActionStatus.Error, DateTime.UtcNow, $"There was an issue handling the action: {e.Message}.");
                _logger.LogError($"ResponseActionEventConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
