using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using Edison.Core.Interfaces;
using MassTransit;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Edison.ResponseService.Consumers
{
    public class ResponseActionEventConsumer : IConsumer<IActionEvent>
    {
        private readonly ILogger<ResponseActionEventConsumer> _logger;

        public ResponseActionEventConsumer(
            ILogger<ResponseActionEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IActionEvent> context)
        {
            try
            {
                _logger.LogDebug($"ResponseActionEventConsumer: Retrieved message from response '{context.Message.Action.ActionId}'.");
                ActionModel action = context.Message.Action;
                if (!string.IsNullOrEmpty(action.ActionType))
                {
                    _logger.LogDebug($"ResponseActionEventConsumer: ActionType retrieved: '{action.ActionType}'.");
                    //Switch statement directing different action types
                    switch(action.ActionType)
                    {
                        case "email":
                            _logger.LogDebug($"ResponseActionEventConsumer: Publish ActionEmailEvent.");
                            await context.Publish(new ActionEmailEvent(action, context.Message.IsCloseAction) { ResponseId = context.Message.ResponseId });
                            break;
                        case "notification":
                            _logger.LogDebug($"ResponseActionEventConsumer: Publish ActionNotificationEvent.");
                            await context.Publish(new ActionNotificationEvent(action, context.Message.IsCloseAction) { ResponseId = context.Message.ResponseId });
                            break;
                        case "lightsensor":
                            _logger.LogDebug($"ResponseActionEventConsumer: Publish ActionLightSensorEvent.");
                            await context.Publish(new ActionLightSensorEvent(action, context.Message.IsCloseAction) {
                                 ResponseId = context.Message.ResponseId,
                                 Epicenter = context.Message.Geolocation,
                                 PrimaryRadius = context.Message.PrimaryRadius,
                                 SecondaryRadius = context.Message.SecondaryRadius
                            });
                            break;
                        case "rapidsos":
                            _logger.LogDebug($"ResponseActionEventConsumer: Publish ActionRapidSOSEvent.");
                            await context.Publish(new ActionRapidSOSEvent(action, context.Message.IsCloseAction) { ResponseId = context.Message.ResponseId });
                            break;
                        default:
                            throw new Exception($"Action Type: '{action.ActionType}' is invalid and unhandled");
                    }
                    return;
                }
                _logger.LogError("ResponseActionEventConsumer: Invalid Null or Empty Action Type");
                throw new Exception("Invalid Null or Empty Action Type");

            }
            catch (Exception e)
            {
                _logger.LogError($"ResponseActionEventConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
