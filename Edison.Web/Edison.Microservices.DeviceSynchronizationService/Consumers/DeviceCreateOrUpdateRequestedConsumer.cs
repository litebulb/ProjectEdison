using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MassTransit;
using Newtonsoft.Json;
using Edison.Core.Interfaces;
using Edison.Core.Common.Models;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;

namespace Edison.DeviceSynchronizationService.Consumers
{
    /// <summary>
    /// Masstransit consumer that handles a creation or modification update coming from IoT Hub
    /// </summary>
    public class DeviceCreateOrUpdateRequestedConsumer : IConsumer<IDeviceCreateOrUpdateRequested>
    {
        private readonly IDeviceRestService _deviceRestService;
        private readonly ILogger<DeviceCreateOrUpdateRequestedConsumer> _logger;

        public DeviceCreateOrUpdateRequestedConsumer(IDeviceRestService deviceRestService, ILogger<DeviceCreateOrUpdateRequestedConsumer> logger)
        {
            _logger = logger;
            _deviceRestService = deviceRestService;
        }

        public async Task Consume(ConsumeContext<IDeviceCreateOrUpdateRequested> context)
        {
            try {
                _logger.LogDebug($"DeviceCreateOrUpdateRequestedConsumer: Retrieved message from device '{context.Message.DeviceId}'");

                if(context.Message.ChangeType == "ping")
                    await ProcessPing(context);
                else
                    await ProcessTwinUpdate(context);
            }
            catch (Exception e)
            {
                _logger.LogError($"DeviceCreateOrUpdateRequestedConsumer: {e.Message}");
                throw e;
            }
        }

        private async Task ProcessPing(ConsumeContext<IDeviceCreateOrUpdateRequested> context)
        {
            var result = await _deviceRestService.UpdateHeartbeat(context.Message.DeviceId);
            if (result != null)
            {
                //Push message to Service bus queue
                _logger.LogDebug($"DeviceCreateOrUpdateRequestedConsumer: Device heartbeat updated with device id '{context.Message.DeviceId}'.");
                await context.Publish(new DeviceCreatedOrUpdatedEvent() { Device = result.Device, CorrelationId = context.Message.CorrelationId, NotifyUI = result.NeedsUpdate });
                return;
            }
            _logger.LogError("DeviceCreateOrUpdateRequestedConsumer: The device heartbeat could not be updated.");
            throw new Exception("The device heartbeat could not be updated.");
        }

        private async Task ProcessTwinUpdate(ConsumeContext<IDeviceCreateOrUpdateRequested> context)
        {
            if (JsonConvert.DeserializeObject<DeviceTwinModel>(context.Message.Data) is DeviceTwinModel obj)
            {
                StripIntervalProperties(obj?.Properties?.Desired);
                StripIntervalProperties(obj?.Properties?.Reported);

                obj.DeviceId = context.Message.DeviceId;

                DeviceModel device = await _deviceRestService.CreateOrUpdateDevice(obj);
                if (device != null && device.DeviceId != Guid.Empty)
                {
                    //Push message to Service bus queue
                    _logger.LogDebug($"DeviceCreateOrUpdateRequestedConsumer: Device created/updated with device id '{context.Message.DeviceId}'.");
                    await context.Publish(new DeviceCreatedOrUpdatedEvent() { Device = device, CorrelationId = context.Message.CorrelationId, NotifyUI = true });
                    return;
                }
                _logger.LogError("DeviceCreateOrUpdateRequestedConsumer: The device could not be updated.");
                throw new Exception("The device could not be updated.");

            }
            _logger.LogError($"DeviceCreateOrUpdateRequestedConsumer: Error while deserializing message '{context.Message.DeviceId}'.");
            throw new Exception($"Error while deserializing message from device '{context.Message.DeviceId}'.");
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
