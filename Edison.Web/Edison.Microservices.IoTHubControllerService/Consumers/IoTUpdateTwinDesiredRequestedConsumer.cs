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
    /// Masstransit consumer that creates a job for IoT Hub to update the desired properties of a set of devices
    /// </summary>
    public class IoTUpdateTwinDesiredRequestedConsumer : IConsumer<IIoTDevicesUpdateRequested>
    {
        private readonly RegistryManagerHelper _registryManager;
        private readonly ILogger<IoTUpdateTwinDesiredRequestedConsumer> _logger;

        public IoTUpdateTwinDesiredRequestedConsumer(RegistryManagerHelper registryManager, ILogger<IoTUpdateTwinDesiredRequestedConsumer> logger)
        {
            _logger = logger;
            _registryManager = registryManager;
        }

        public async Task Consume(ConsumeContext<IIoTDevicesUpdateRequested> context)
        {
            try
            {
                _logger.LogDebug($"IoTUpdateTwinDesiredRequestedConsumer: Retrieved message from {context.Message.DeviceIds.Count} devices.");

                var result = await _registryManager.UpdateDevices(context.Message.DeviceIds, context.Message.JsonTags, context.Message.JsonDesired, context.Message.WaitForCompletion);
                if (result)
                {
                    //Push message to Service bus queue
                    _logger.LogDebug($"IoTUpdateTwinDesiredRequestedConsumer: {context.Message.DeviceIds.Count} devices updated.");
                    await context.RespondAsync(new IoTDevicesUpdatedEvent() { DeviceIds = context.Message.DeviceIds });
                    return;
                }
                _logger.LogError($"IoTUpdateTwinDesiredRequestedConsumer: {context.Message.DeviceIds.Count} devices could not be updated.");
                throw new Exception($"{context.Message.DeviceIds.Count} devices could not be updated.");

            }
            catch (Exception e)
            {
                _logger.LogError($"IoTUpdateTwinDesiredRequestedConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
