using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MassTransit;
using Edison.Core.Interfaces;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;

namespace Edison.SignalRService.Consumers
{
    /// <summary>
    /// Masstransit consumer that handles a signal R UI update for Event update message
    /// </summary>
    public class EventUIUpdateConsumer : IConsumer<IEventUIUpdateRequested>
    {
        private readonly ISignalRRestService _signalsRestService;
        private readonly ILogger<EventUIUpdateConsumer> _logger;

        public EventUIUpdateConsumer(ISignalRRestService signalsRestService, ILogger<EventUIUpdateConsumer> logger)
        {
            _logger = logger;
            _signalsRestService = signalsRestService;
        }

        public async Task Consume(ConsumeContext<IEventUIUpdateRequested> context)
        {
            _logger.LogDebug($"EventUIUpdateConsumer: Retrieved message: '{context.Message.CorrelationId}'");

            if (await _signalsRestService.UpdateEventClusterUI(context.Message.EventClusterUI))
            {
                _logger.LogDebug($"EventUIUpdateConsumer: Message sent successfully.");
                await context.Publish(new EventUIUpdatedEvent() { CorrelationId = context.Message.CorrelationId });
            }
            else
                _logger.LogError($"EventUIUpdateConsumer: Error while sending a message.");
        }
    }
}
