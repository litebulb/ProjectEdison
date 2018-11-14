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
