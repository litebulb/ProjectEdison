using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MassTransit;
using Edison.Core.Interfaces;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;

namespace Edison.SignalRService.Consumers
{
    /// <summary>
    /// Masstransit consumer that handles a signal R UI update for Device update message
    /// </summary>
    public class DeviceUIUpdateConsumer : IConsumer<IDeviceUIUpdateRequested>
    {
        private readonly ISignalRRestService _signalsRestService;
        private readonly ILogger<DeviceUIUpdateConsumer> _logger;

        public DeviceUIUpdateConsumer(ISignalRRestService signalsRestService, ILogger<DeviceUIUpdateConsumer> logger)
        {
            _logger = logger;
            _signalsRestService = signalsRestService;
        }

        public async Task Consume(ConsumeContext<IDeviceUIUpdateRequested> context)
        {
            _logger.LogDebug($"EventUIUpdateConsumer: Retrieved message: '{context.Message.CorrelationId}'");

            if (await _signalsRestService.UpdateDeviceUI(context.Message.DeviceUI))
            {
                _logger.LogDebug($"DeviceUIUpdateConsumer: Message sent successfully.");
                await context.Publish(new DeviceUIUpdatedEvent() { CorrelationId = context.Message.CorrelationId });
            }
            else
                _logger.LogError($"DeviceUIUpdateConsumer: Error while sending a message.");
        }
    }
}
