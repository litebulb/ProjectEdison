using Edison.Simulators.Sensors.Config;
using Edison.Simulators.Sensors.Models;
using Edison.Simulators.Sensors.Models.Helpers;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Simulators.Sensors.Helpers
{
    public class IoTDeviceHelper
    {
        private readonly RegistryManager _registryManager;
        private readonly JobClient _jobClient;
        private readonly SimulatorConfig _config;
        private List<IoTDevice> _CacheDevices = null;

        public IoTDeviceHelper(IOptions<SimulatorConfig> config)
        {
            _config = config.Value;
            _registryManager = RegistryManager.CreateFromConnectionString(_config.IoTHubConnectionString);
            _jobClient = JobClient.CreateFromConnectionString(_config.IoTHubConnectionString);
        }

        public async Task CreateDevice(IoTDevice newDevice, bool overrideTags)
        {
            //reset cache
            await EmptyCacheDevices();

            Device device = await _registryManager.GetDeviceAsync(newDevice.DeviceId);
            if (device == null)
            {
                ConsoleHelper.WriteInfo($"Create new demo device '{newDevice.DeviceId}'");
                device = await _registryManager.AddDeviceAsync(new Device(newDevice.DeviceId));
            }
            else
            {
                if(overrideTags)
                    ConsoleHelper.WriteInfo($"The device '{newDevice.DeviceId}' already exist. Updating tags.");
                else
                {
                    ConsoleHelper.WriteInfo($"The device '{newDevice.DeviceId}' already exist. Skipping.");
                    return;
                }
            }

            Twin twin = new Twin
            {
                Tags = new TwinCollection()
            };
            if (newDevice.Demo)
                twin.Tags["Demo"] = newDevice.Demo;
            twin.Tags["Enabled"] = newDevice.Enabled;
            twin.Tags["Sensor"] = newDevice.Sensor;
            twin.Tags["DeviceType"] = newDevice.DeviceType;
            twin.Tags["Name"] = newDevice.Name;
            twin.Tags["Location1"] = newDevice.Location1;
            twin.Tags["Location2"] = newDevice.Location2;
            twin.Tags["Location3"] = newDevice.Location3;
            twin.Tags["Geolocation"] = new Geolocation()
            {
                Latitude = newDevice.Latitude,
                Longitude = newDevice.Longitude
            };
            twin.ETag = "*";

            await _registryManager.UpdateTwinAsync(newDevice.DeviceId, twin, twin.ETag);
        }

        private async Task<List<IoTDevice>> GetDevices()
        {
            if (_CacheDevices != null)
                return _CacheDevices;

            List<IoTDevice> iotDevices = new List<IoTDevice>();
            IQuery query = _registryManager.CreateQuery(
                "SELECT deviceId, " +
                "tags.Demo as Demo, " +
                "tags.Sensor as Sensor, " +
                "tags.Enabled as Enabled, " +
                "tags.DeviceType as DeviceType, " +
                "tags.Name as Name, " +
                "tags.Location1 as Location1, " +
                "tags.Location2 as Location2, " +
                "tags.Location3 as Location3, " +
                "tags.Geolocation.Latitude as Latitude, " +
                "tags.Geolocation.Longitude as Longitude, " +
                "properties.desired " +
                "FROM devices");
            while (query.HasMoreResults)
            {
                var page = await query.GetNextAsJsonAsync();
                foreach (var result in page)
                {
                    var device = JsonConvert.DeserializeObject<IoTDevice>(result);
                    iotDevices.Add(device);
                }
            }

            foreach (var iotDevice in iotDevices)
            {
                iotDevice.Client = DeviceClient.CreateFromConnectionString(_config.IoTHubConnectionString, iotDevice.DeviceId);
                //Removed because cause issues with running physical devices.
                //if(!iotDevice.Sensor)
                //    await iotDevice.Client.SetDesiredPropertyUpdateCallbackAsync(ReceiveDesiredConfiguration, iotDevice.DeviceId);
            }

            _CacheDevices = iotDevices;
            return iotDevices;
        }

        private async Task ReceiveDesiredConfiguration(TwinCollection desiredProperties, object userContext)
        {
            var deviceOutput = await GetAllOutputDevices();
            var device = deviceOutput.Find(p => p.DeviceId == userContext.ToString());
            device.Desired = JsonConvert.DeserializeObject<Dictionary<string,object>>(desiredProperties.ToString());
        }

        public async Task<List<IoTDevice>> GetDemoDevices()
        {
            List<IoTDevice> devices = await GetDevices();
            return devices.Where(p => p.Demo).ToList();
        }

        public async Task<List<IoTDevice>> GetAllInputDevices()
        {
            List<IoTDevice> devices = await GetDevices();
            return devices.Where(p => p.Sensor && p.Enabled).OrderBy(p => p.Longitude).ThenByDescending(p => p.Latitude).ToList();
        }

        public async Task<List<IoTDevice>> GetAllOutputDevices()
        {
            List<IoTDevice> devices = await GetDevices();
            return devices.Where(p => !p.Sensor && p.Enabled).OrderBy(p => p.Longitude).ThenByDescending(p => p.Latitude).ToList();
        }

        public async Task DeleteMultipleDevicesAsync(List<IoTDevice> iotDevices)
        {
            try
            {
                //reset cache
                await EmptyCacheDevices();

                ConsoleHelper.WriteInfo($"Deleting {iotDevices.Count} demo devices...");
                List<Device> devices = new List<Device>();
                int i = 0;
                foreach (var iotDevice in iotDevices)
                {
                    Device device = new Device(iotDevice.DeviceId);
                    if (device != null)
                    {
                        devices.Add(device);
                        i++;
                    }
                    if (i >= 50)
                    {
                        ConsoleHelper.WriteInfo($"Delete will be limited to 50 devices. Run the command again to delete more devices.");
                        break;
                    }
                }

                if (devices.Count > 0)
                    await _registryManager.RemoveDevices2Async(devices, true, new System.Threading.CancellationToken());
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteError($"Delete multiple devices error {e.Message}");
                throw e;
            }
        }

        public async Task SendMessage(IoTDevice device, string eventType, object message)
        {
            string messageJson = JsonConvert.SerializeObject(message);

            var messageIoT = new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(messageJson));
            messageIoT.Properties.Add("opType", "eventDevice");
            messageIoT.Properties.Add("eventType", eventType);
            await device.Client.SendEventAsync(messageIoT);
        }

        private async Task EmptyCacheDevices()
        {
            if (_CacheDevices == null)
                return;

            foreach(var device in _CacheDevices)
            {
                if (device.Client != null)
                {
                    //await iotDevice.Client.RemoveDesiredPropertyUpdatesAsync(null, null); //TODO: uncomment when device sdk will allow unregistering
                    await device.Client.CloseAsync();
                    device.Client.Dispose();
                }
            }
            _CacheDevices = null;
        }
    }
}
