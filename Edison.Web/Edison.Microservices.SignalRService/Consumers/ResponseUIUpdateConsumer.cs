using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MassTransit;
using Edison.Core.Interfaces;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;

namespace Edison.SignalRService.Consumers
{
    /// <summary>
    /// Masstransit consumer that handles a signal R UI update for Response update message
    /// </summary>
    public class ResponseUIUpdateConsumer : IConsumer<IResponseUIUpdateRequested>
    {
        private readonly ISignalRRestService _signalsRestService;
        private readonly ILogger<ResponseUIUpdateConsumer> _logger;

        public ResponseUIUpdateConsumer(ISignalRRestService signalsRestService, ILogger<ResponseUIUpdateConsumer> logger)
        {
            _logger = logger;
            _signalsRestService = signalsRestService;
        }

        public async Task Consume(ConsumeContext<IResponseUIUpdateRequested> context)
        {
            _logger.LogDebug($"ResponseUIUpdateConsumer: Retrieved message: '{context.Message.ResponseId}'");

            if (await _signalsRestService.UpdateResponseUI(context.Message.ResponseUI))
            {
                _logger.LogDebug($"ResponseUIUpdateConsumer: Message sent successfully.");
                await context.Publish(new ResponseUIUpdatedEvent() { ResponseId = context.Message.ResponseId });
            }
            else
                _logger.LogError($"ResponseUIUpdateConsumer: Error while sending a message.");
        }
    }
}
