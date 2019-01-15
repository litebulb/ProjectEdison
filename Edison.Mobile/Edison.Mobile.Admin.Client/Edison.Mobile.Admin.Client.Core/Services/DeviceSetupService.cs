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
        public static class DeviceTypeValues
        {
            public const string ButtonSensor = "Edison.Devices.ButtonSensor";
            public const string SmartBulb = "Edison.Devices.SmartBulb";
            public const string SoundSensor = "Edison.Devices.SoundSensor";
        }

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

        public string DefaultPassword => "Edison1234";

        public string DeviceTypeAsString => DeviceTwinModel?.Tags?.DeviceType ?? "";
        public string DeviceTypeAsFriendlyString => DeviceTypeToFriendlyString(DeviceTypeAsString);

        public DeviceSetupService(IWifiService wifiService)
        {
            this.wifiService = wifiService;
        }

        public void ClearDevice()
        {
            DeviceTwinModel = null;
            CurrentDeviceHotspotNetwork = null;
        }

        public void SetDeviceType(DeviceType deviceType)
        {
            DeviceTwinModel.Tags.DeviceType = DeviceTypeToString(deviceType);
        }

        public void SetDeviceGuid(Guid guid)
        {
            DeviceTwinModel.DeviceId = guid;
        }

        public static string DeviceTypeToString(DeviceType deviceType)
        {
            switch (deviceType)
            {
                case DeviceType.Button:
                    return DeviceTypeValues.ButtonSensor;
                case DeviceType.Light:
                    return DeviceTypeValues.SmartBulb;
                default:
                    return DeviceTypeValues.SoundSensor;
            }
        }

        public static string DeviceTypeToFriendlyString(string deviceType)
        {
            switch (deviceType)
            {
                case DeviceTypeValues.ButtonSensor:
                    return "Button";
                case DeviceTypeValues.SmartBulb:
                    return "Light";
                default:
                    return DeviceTypeValues.SoundSensor;
            }
        }
    }
}