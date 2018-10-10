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
    public class ResponseActionRapidSOSEventConsumer : IConsumer<IActionRapidSOSEvent>
    {
        private readonly ILogger<ResponseActionRapidSOSEventConsumer> _logger;

        public ResponseActionRapidSOSEventConsumer(
            ILogger<ResponseActionRapidSOSEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IActionRapidSOSEvent> context)
        {
            try
            {
                if (context.Message != null && context.Message as IActionRapidSOSEvent != null)
                {
                    IActionRapidSOSEvent action = context.Message;
                    _logger.LogDebug($"ResponseActionRapidSOSEventConsumer: ActionId: '{action.ActionId}'.");
                    _logger.LogDebug($"ResponseActionRapidSOSEventConsumer: Message: '{action.Message}'.");
                    _logger.LogDebug($"ResponseActionRapidSOSEventConsumer: ServiceType: '{action.ServiceType}'.");
                    //await new Task(() => { throw new NotImplementedException("Need ResponseActionRapidSOSEventConsumer Logic"); });
                    await Task.Delay(1000);
                }
                _logger.LogError("ResponseActionRapidSOSEventConsumer: Invalid Null or Empty Action RapidSOS");
                throw new Exception("Invalid or Null Action RapidSOS");

            }
            catch (Exception e)
            {
                _logger.LogError($"ResponseActionRapidSOSEventConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
