using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Core.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;

namespace Edison.SignalRService.Consumers
{
    public class ActionCloseUIUpdateConsumer : IConsumer<IActionCloseUIUpdatedRequested>
    {
        private readonly ISignalRRestService _signalsRestService;
        private readonly ILogger<ActionCloseUIUpdateConsumer> _logger;

        public ActionCloseUIUpdateConsumer(ISignalRRestService signalsRestService, ILogger<ActionCloseUIUpdateConsumer> logger)
        {
            _logger = logger;
            _signalsRestService = signalsRestService;
        }

        public async Task Consume(ConsumeContext<IActionCloseUIUpdatedRequested> context)
        {
            _logger.LogDebug($"ActionCloseUIUpdateConsumer: Retrieved message: Response='{context.Message.ResponseId}' Action='{context.Message.ActionId}'");

            if (await _signalsRestService.UpdateActionCloseUI(context.Message.ActionCloseModel))
            {
                _logger.LogDebug($"ActionCloseUIUpdateConsumer: Message sent successfully.");
                await context.Publish(new ActionCloseUIUpdatedEvent() { ResponseId = context.Message.ResponseId, ActionId = context.Message.ActionId });
            }
            else
                _logger.LogError($"ActionCloseUIUpdateConsumer: Error while sending a message.");
        }
    }
}
