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
