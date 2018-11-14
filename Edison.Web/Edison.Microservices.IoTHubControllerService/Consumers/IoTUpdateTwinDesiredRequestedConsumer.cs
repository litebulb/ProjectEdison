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
