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

        public WifiNetwork CurrentWifiNetwork { get; set; }
        public WifiNetwork CurrentDeviceHotspotNetwork { get; set; }

        public DeviceSetupService(IWifiService wifiService)
        {
            this.wifiService = wifiService;
        }

        public string DeviceTypeAsString => DeviceTwinModel?.Tags?.DeviceType ?? "";

        public void ClearDevice()
        {
            DeviceTwinModel = null;
            CurrentDeviceHotspotNetwork = null;
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
    }
}