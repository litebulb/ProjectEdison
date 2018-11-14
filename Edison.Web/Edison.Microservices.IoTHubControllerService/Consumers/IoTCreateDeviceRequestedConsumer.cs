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
using Microsoft.Azure.Devices;
using Edison.IoTHubControllerService.Helpers;

namespace Edison.IoTHubControllerService.Consumers
{
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
