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
    public class ResponseActionEmailEventConsumer : IConsumer<IActionEmailEvent>
    {
        private readonly ILogger<ResponseActionEmailEventConsumer> _logger;

        public ResponseActionEmailEventConsumer(
            ILogger<ResponseActionEmailEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IActionEmailEvent> context)
        {
            try
            {
                if (context.Message != null && context.Message as IActionEmailEvent != null)
                {
                    IActionEmailEvent action = context.Message;
                    _logger.LogDebug($"ResponseActionEmailEventConsumer: ActionId: '{action.ActionId}'.");
                    _logger.LogDebug($"ResponseActionEmailEventConsumer: Subject: '{action.Subject}'.");
                    _logger.LogDebug($"ResponseActionEmailEventConsumer: ToLine: '{action.ToLine}'.");
                    _logger.LogDebug($"ResponseActionEmailEventConsumer: CCLine: '{action.CCLine}'.");
                    _logger.LogDebug($"ResponseActionEmailEventConsumer: Body: '{action.Body}'.");
                    //await new Task(() => { throw new NotImplementedException("Need ResponseActionEmailEventConsumer Logic"); });
                    await context.Publish(new EventSagaReceiveResponseActionClosed(context.Message.IsCloseAction)
                    {
                        ResponseId = context.Message.ResponseId,
                        ActionId = context.Message.ActionId,
                        IsSuccessful = true
                    });
                    await Task.Delay(1000);
                }
                _logger.LogError("ResponseActionEmailEventConsumer: Invalid Null or Empty Action Email");
                throw new Exception("Invalid or Null Action Email");

            }
            catch (Exception e)
            {
                await context.Publish(new EventSagaReceiveResponseActionClosed(context.Message.IsCloseAction)
                {
                    ResponseId = context.Message.ResponseId,
                    ActionId = context.Message.ActionId,
                    IsSuccessful = false
                });
                _logger.LogError($"ResponseActionEmailEventConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
