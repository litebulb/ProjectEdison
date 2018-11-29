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
    /// Masstransit consumer that handles the creation of an IoT Hub device.
    /// This consumer is meant to be used for debugging only. The devices should be self provisioned.
    /// </summary>
    public class IoTCreateDeviceRequestedConsumer : IConsumer<IIoTDeviceCreateRequested>
    {
        private readonly RegistryManagerHelper _registryManager;
        private readonly ILogger<IoTCreateDeviceRequestedConsumer> _logger;

        public IoTCreateDeviceRequestedConsumer(RegistryManagerHelper registryManager, ILogger<IoTCreateDeviceRequestedConsumer> logger)
        {
            _logger = logger;
            _registryManager = registryManager;
        }

        public async Task Consume(ConsumeContext<IIoTDeviceCreateRequested> context)
        {
            try {
                _logger.LogDebug($"IoTCreateDeviceRequestedConsumer: Retrieved message from device '{context.Message.DeviceId}'");

                var result = await _registryManager.CreateDevice(context.Message.DeviceId, context.Message.JsonTags, context.Message.JsonDesired);
                if (result)
                {
                    //Push message to Service bus queue
                    _logger.LogDebug($"IoTCreateDeviceRequestedConsumer: Device created with device id '{context.Message.DeviceId}'.");
                    await context.RespondAsync(new IoTDeviceCreatedEvent(){ DeviceId = context.Message.DeviceId });
                    return;
                }
                _logger.LogError("IoTCreateDeviceRequestedConsumer: The device could not be created.");
                throw new Exception("The device could not be created.");

            }
            catch (Exception e)
            {
                _logger.LogError($"IoTCreateDeviceRequestedConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
