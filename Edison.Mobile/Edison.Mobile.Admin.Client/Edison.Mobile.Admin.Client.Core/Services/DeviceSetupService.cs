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

        DeviceModel deviceModel;

        public WifiNetwork CurrentWifiNetwork { get; set; }
        public WifiNetwork CurrentDeviceHotspotNetwork { get; set; }

        public string DefaultPassword => "Edison1234";

        public string DeviceTypeAsString => deviceModel?.DeviceType ?? "";
        public string DeviceTypeAsFriendlyString => DeviceTypeToFriendlyString(DeviceTypeAsString);

        public void ClearDevice()
        {
            deviceModel = null;
            CurrentDeviceHotspotNetwork = null;
        }

        public void SetDeviceType(DeviceType deviceType)
        {
            EnsureDeviceExists();
            deviceModel.DeviceType = DeviceTypeToString(deviceType);
        }

        public void SetDeviceGuid(Guid guid)
        {
            EnsureDeviceExists();
            deviceModel.DeviceId = guid;
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

        public void AddCustomDeviceField(string key, string value)
        {
            if (deviceModel.Custom == null)
            {
                deviceModel.Custom = new Dictionary<string, object>();
            }

            deviceModel.Custom[key] = value;
        }

        public static DeviceType DeviceTypeFromString(string deviceType)
        {
            switch (deviceType)
            {
                case DeviceTypeValues.ButtonSensor:
                    return DeviceType.Button;
                case DeviceTypeValues.SmartBulb:
                    return DeviceType.Light;
                default:
                    return DeviceType.SoundSensor;
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

        void EnsureDeviceExists()
        {
            if (deviceModel == null) deviceModel = new DeviceModel();
        }
    }
}