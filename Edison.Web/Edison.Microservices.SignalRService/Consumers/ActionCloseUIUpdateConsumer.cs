using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MassTransit;
using Edison.Core.Interfaces;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;

namespace Edison.SignalRService.Consumers
{
    /// <summary>
    /// Masstransit consumer that handles a signal R UI update for Action Close message
    /// </summary>
    public class ActionCloseUIUpdateConsumer : IConsumer<IActionCallbackUIUpdatedRequested>
    {
        private readonly ISignalRRestService _signalsRestService;
        private readonly ILogger<ActionCloseUIUpdateConsumer> _logger;

        public ActionCloseUIUpdateConsumer(ISignalRRestService signalsRestService, ILogger<ActionCloseUIUpdateConsumer> logger)
        {
            _logger = logger;
            _signalsRestService = signalsRestService;
        }

        public async Task Consume(ConsumeContext<IActionCallbackUIUpdatedRequested> context)
        {
            _logger.LogDebug($"ActionCloseUIUpdateConsumer: Retrieved message: Response='{context.Message.ResponseId}' Action='{context.Message.ActionId}'");

            if (await _signalsRestService.UpdateActionCloseUI(context.Message.ActionCloseModel))
            {
                _logger.LogDebug($"ActionCloseUIUpdateConsumer: Message sent successfully.");
                await context.Publish(new ActionCallbackUIUpdatedEvent() { ResponseId = context.Message.ResponseId, ActionId = context.Message.ActionId });
            }
            else
                _logger.LogError($"ActionCloseUIUpdateConsumer: Error while sending a message.");
        }
    }
}
