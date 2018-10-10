using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using MassTransit;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Edison.IoTHubControllerService.Helpers;

namespace Edison.IoTHubControllerService.Consumers
{
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
