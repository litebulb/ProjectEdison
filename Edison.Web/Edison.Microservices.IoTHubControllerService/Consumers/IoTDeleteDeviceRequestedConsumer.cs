using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MassTransit;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;
using Edison.IoTHubControllerService.Helpers;

namespace Edison.IoTHubControllerService.Consumers
{
    /// <summary>
    /// Masstransit consumer that handles the deletion of an IoT Hub device.
    /// </summary>
    public class IoTDeleteDeviceRequestedConsumer : IConsumer<IIoTDeviceDeleteRequested>
    {
        private readonly RegistryManagerHelper _registryManager;
        private readonly ILogger<IoTDeleteDeviceRequestedConsumer> _logger;

        public IoTDeleteDeviceRequestedConsumer(RegistryManagerHelper registryManager, ILogger<IoTDeleteDeviceRequestedConsumer> logger)
        {
            _logger = logger;
            _registryManager = registryManager;
        }

        public async Task Consume(ConsumeContext<IIoTDeviceDeleteRequested> context)
        {
            try
            {
                _logger.LogDebug($"IoTDeleteDeviceRequestedConsumer: Retrieved message from device '{context.Message.DeviceId}'");

                var result = await _registryManager.DeleteDevice(context.Message.DeviceId);
                if (result)
                {
                    //Push message to Service bus queue
                    _logger.LogDebug($"IoTDeleteDeviceRequestedConsumer: Device deleted with device id '{context.Message.DeviceId}'.");
                    await context.RespondAsync(new IoTDeviceDeletedEvent() { DeviceId = context.Message.DeviceId });
                    return;
                }
                _logger.LogError("IoTDeleteDeviceRequestedConsumer: The device could not be deleted.");
                throw new Exception("The device could not be deleted.");

            }
            catch (Exception e)
            {
                _logger.LogError($"IoTDeleteDeviceRequestedConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
