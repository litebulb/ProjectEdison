using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Edison.Core.Common.Models;
using Edison.Common.Interfaces;
using Edison.Common.Messages;

namespace Edison.Api.Helpers
{
    /// <summary>
    /// Manager for the Iot Hub registry service
    /// </summary>
    public class IoTHubControllerDataManager
    {
        private readonly IMassTransitServiceBus _serviceBus;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public IoTHubControllerDataManager(IMassTransitServiceBus serviceBus)
        {
            _serviceBus = serviceBus;
        }

        /// <summary>
        /// Create a device
        /// </summary>
        /// <param name="device">DeviceCreationModel</param>
        /// <returns>True if the masstransit publish command has succeeded</returns>
        public async Task<bool> CreationDevice(DeviceCreationModel device)
        {
            var tags = new
            {
                device.DeviceType,
                device.Sensor,
                device.Geolocation,
                device.Name,
                device.Location1,
                device.Location2,
                device.Location3,
                device.Custom
            };

            await _serviceBus.BusAccess.Publish(new IoTDeviceCreateRequestedEvent()
            {
                DeviceId = device.DeviceId,
                JsonDesired = JsonConvert.SerializeObject(device.Desired),
                JsonTags = JsonConvert.SerializeObject(tags)
            });
            return true;
        }

        /// <summary>
        /// Update a device
        /// </summary>
        /// <param name="device">DeviceUpdateModel</param>
        /// <returns>True if the masstransit publish command has succeeded</returns>
        public async Task<bool> UpdateDevice(DeviceUpdateModel device)
        {
            var tags = new
            {
                device.Geolocation,
                device.Name,
                device.Location1,
                device.Location2,
                device.Location3,
                device.Custom,
                device.Enabled
            };

            await _serviceBus.BusAccess.Publish(new IoTDevicesUpdateRequestedEvent()
            {
                DeviceIds = new List<Guid>() { device.DeviceId },
                JsonDesired = JsonConvert.SerializeObject(device.Desired),
                JsonTags = JsonConvert.SerializeObject(tags)
            });
            return true;
        }

        /// <summary>
        /// Update a set of devices tags
        /// </summary>
        /// <param name="devices">List of device ids</param>
        /// <returns>True if the masstransit publish command has succeeded</returns>
        public async Task<bool> UpdateDevicesTags(DevicesUpdateTagsModel devices)
        {
            var tags = new DeviceTwinTagsModel
            {
                Geolocation = devices.Geolocation,
                Name = devices.Name,
                Location1 = devices.Location1,
                Location2 = devices.Location2,
                Location3 = devices.Location3,
                Custom = devices.Custom,
                Enabled = devices.Enabled,
                SSID = devices.SSID
            };

            await _serviceBus.BusAccess.Publish(new IoTDevicesUpdateRequestedEvent()
            {
                DeviceIds = devices.DeviceIds,
                JsonDesired = "",
                JsonTags = JsonConvert.SerializeObject(tags)
            });
            return true;
        }

        /// <summary>
        /// Update a set of devices desired properties
        /// </summary>
        /// <param name="devices">List of device ids</param>
        /// <returns>True if the masstransit publish command has succeeded</returns>
        public async Task<bool> UpdateDevicesDesired(DevicesUpdateDesiredModel devices)
        {
            await _serviceBus.BusAccess.Publish(new IoTDevicesUpdateRequestedEvent()
            {
                DeviceIds = devices.DeviceIds,
                JsonDesired = JsonConvert.SerializeObject(devices.Desired),
                JsonTags = string.Empty,
                WaitForCompletion = true
            });
            return true;
        }

        /// <summary>
        /// Launch a direct method on a set of devices
        /// </summary>
        //// <param name="devices">List of device ids</param>
        /// <returns>True if the masstransit publish command has succeeded</returns>
        public async Task<bool> LaunchDevicesDirectMethods(DevicesLaunchDirectMethodModel devices)
        {
            await _serviceBus.BusAccess.Publish(new IoTDevicesDirectMethodRequestedEvent()
            {
                DeviceIds = devices.DeviceIds,
                MethodName = devices.MethodName,
                MethodPayload = devices.Payload
            });
            return true;
        }

        /// <summary>
        /// Delete a device
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <returns>True if the masstransit publish command has succeeded</returns>
        public async Task<bool> DeleteDevice(Guid deviceId)
        {
            await _serviceBus.BusAccess.Publish(new IoTDeviceDeleteRequestedEvent()
            {
                DeviceId = deviceId
            });
            return true;
        }
    }
}
