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
    public class ResponseActionNotificationEventConsumer : IConsumer<IActionNotificationEvent>
    {
        private readonly ILogger<ResponseActionNotificationEventConsumer> _logger;

        public ResponseActionNotificationEventConsumer(
            ILogger<ResponseActionNotificationEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IActionNotificationEvent> context)
        {
            try
            {
                if (context.Message != null && context.Message as IActionNotificationEvent != null)
                {
                    IActionNotificationEvent action = context.Message;
                    _logger.LogDebug($"ResponseActionNotificationEventConsumer: ActionId: '{action.ActionId}'.");
                    _logger.LogDebug($"ResponseActionNotificationEventConsumer: PhoneNumber: '{action.PhoneNumber}'.");
                    _logger.LogDebug($"ResponseActionNotificationEventConsumer: Message: '{action.Message}'.");
                    _logger.LogDebug($"ResponseActionNotificationEventConsumer: IsSilent: '{action.IsSilent}'.");
                    //await new Task(() => { throw new NotImplementedException("Need ResponseActionNotificationEventConsumer Logic"); });
                    await Task.Delay(1000);
                }
                _logger.LogError("ResponseActionNotificationEventConsumer: Invalid Null or Empty Action Notification");
                throw new Exception("Invalid or Null Action Notification");

            }
            catch (Exception e)
            {
                _logger.LogError($"ResponseActionNotificationEventConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
