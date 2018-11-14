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
    public class IoTLaunchDirectMethodRequestedConsumer : IConsumer<IIoTDevicesDirectMethodRequested>
    {
        private readonly RegistryManagerHelper _registryManager;
        private readonly ILogger<IoTLaunchDirectMethodRequestedConsumer> _logger;

        public IoTLaunchDirectMethodRequestedConsumer(RegistryManagerHelper registryManager, ILogger<IoTLaunchDirectMethodRequestedConsumer> logger)
        {
            _logger = logger;
            _registryManager = registryManager;
        }

        public async Task Consume(ConsumeContext<IIoTDevicesDirectMethodRequested> context)
        {
            try
            {
                _logger.LogDebug($"IoTLaunchDirectMethodRequestedConsumer: Retrieved message from {context.Message.DeviceIds.Count} devices.");

                var result = await _registryManager.LaunchDirectMethods(context.Message.DeviceIds, context.Message.MethodName, context.Message.MethodPayload, context.Message.WaitForCompletion);
                if (result)
                {
                    //Push message to Service bus queue
                    _logger.LogDebug($"IoTLaunchDirectMethodRequestedConsumer: {context.Message.DeviceIds.Count} devices have launched direct method '{context.Message.MethodName}'.");
                    await context.RespondAsync(new IoTDevicesUpdatedEvent() { DeviceIds = context.Message.DeviceIds });
                    return;
                }
                _logger.LogError($"IoTLaunchDirectMethodRequestedConsumer: {context.Message.DeviceIds.Count} devices could not launch direct method '{context.Message.MethodName}'.");
                throw new Exception($"{context.Message.DeviceIds.Count} devices could not launch direct method '{context.Message.MethodName}'.");

            }
            catch (Exception e)
            {
                _logger.LogError($"IoTLaunchDirectMethodRequestedConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
