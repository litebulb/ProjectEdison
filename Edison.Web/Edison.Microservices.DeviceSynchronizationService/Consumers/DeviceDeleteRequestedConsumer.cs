using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using Edison.Core.Interfaces;
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

namespace Edison.DeviceSynchronizationService.Consumers
{
    public class DeviceDeleteRequestedConsumer : IConsumer<IDeviceDeleteRequested>
    {
        private readonly IDeviceRestService _deviceRestService;
        private readonly ILogger<DeviceDeleteRequestedConsumer> _logger;

        public DeviceDeleteRequestedConsumer(IDeviceRestService deviceRestService, ILogger<DeviceDeleteRequestedConsumer> logger)
        {
            _logger = logger;
            _deviceRestService = deviceRestService;
        }

        public async Task Consume(ConsumeContext<IDeviceDeleteRequested> context)
        {
            try {
                _logger.LogDebug($"DeviceDeleteRequestedConsumer: Retrieved message from device '{context.Message.DeviceId}'");

                if (await _deviceRestService.DeleteDevice(context.Message.DeviceId))
                {
                    _logger.LogDebug($"DeviceDeleteRequestedConsumer: Device '{context.Message.DeviceId}' deleted.");
                    await context.RespondAsync(new DeviceDeletedEvent() { DeviceId = context.Message.DeviceId });
                    return;
                }
                _logger.LogError("DeviceDeleteRequestedConsumer: The device could not be deleted.");
                throw new Exception("The device could not be deleted.");
            }
            catch (Exception e)
            {
                _logger.LogError($"DeviceDeleteRequestedConsumer: {e.Message}");
                throw e;
            }
        }

        private async Task<DeviceModel> CreateOrUpdateDeviceTwinReplication(IDeviceCreateOrUpdateRequested message)
        {
            if (JsonConvert.DeserializeObject<DeviceTwinModel>(message.Data) is DeviceTwinModel obj)
            {
                StripIntervalProperties(obj?.Properties?.Desired);
                StripIntervalProperties(obj?.Properties?.Reported);

                obj.DeviceId = message.DeviceId;

                return await _deviceRestService.CreateOrUpdateDevice(obj);
                
            }
            return null;
        }

        private async Task<DeviceModel> DeleteDeviceTwinReplication(IDeviceCreateOrUpdateRequested message)
        {
            if (JsonConvert.DeserializeObject<DeviceTwinModel>(message.Data) is DeviceTwinModel obj)
            {
                await Task.Delay(200);

            }
            return new DeviceModel();
        }

        private void StripIntervalProperties(Dictionary<string, object> properties)
        {
            if (properties == null)
                return;

            if (properties.ContainsKey("$metadata"))
                properties.Remove("$metadata");
            if (properties.ContainsKey("$version"))
                properties.Remove("$version");
        }
    }
}
