using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.Shared;
using Edison.Mobile.Common.WiFi;
using System.Linq;

namespace Edison.Mobile.Admin.Client.Core.Services
{
    public class DeviceSetupService
    {
        readonly IWifiService wifiService;

        DeviceTwinModel deviceTwinModel;
        DeviceTwinModel DeviceTwinModel
        {
            get => deviceTwinModel ?? (deviceTwinModel = new DeviceTwinModel
            {
                Tags = new DeviceTwinTagsModel(),
                Properties = new DeviceTwinPropertiesModel()
            });
            set => deviceTwinModel = value;
        }

        public WifiNetwork DeviceHotspotNetwork { get; set; }

        public DeviceSetupService(IWifiService wifiService)
        {
            this.wifiService = wifiService;
        }

        public string DeviceTypeAsString => DeviceTwinModel?.Tags?.DeviceType ?? "";

        public void ClearDevice()
        {
            DeviceTwinModel = null;
            DeviceHotspotNetwork = null;
        }

        public void SetDeviceType(DeviceType deviceType)
        {
            switch (deviceType)
            {
                case DeviceType.Button:
                    DeviceTwinModel.Tags.DeviceType = "Button";
                    break;
                case DeviceType.Light:
                    DeviceTwinModel.Tags.DeviceType = "Light";
                    break;
                case DeviceType.SoundSensor:
                    DeviceTwinModel.Tags.DeviceType = "Sound Sensor";
                    break;
            }
        }

        public void SetDeviceGuid(Guid guid)
        {
            DeviceTwinModel.DeviceId = guid;
        }

        public async Task<List<WifiNetwork>> GetAvailableWifiNetworks()
        {
            var wifiNetworks = await wifiService.GetWifiNetworks();
            return wifiNetworks?.ToList() ?? new List<WifiNetwork>();
        }

        public async Task<bool> ConnectToDeviceHotspot(WifiNetwork wifiNetwork)
        {
            var success = await wifiService.ConnectToSecuredNetwork(wifiNetwork.SSID, "Edison1234");
            DeviceHotspotNetwork = success ? wifiNetwork : DeviceHotspotNetwork;
            return success;
        }
    }
}
