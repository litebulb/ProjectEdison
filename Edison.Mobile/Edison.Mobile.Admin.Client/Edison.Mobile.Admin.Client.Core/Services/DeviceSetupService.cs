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
        public bool IsNew { get; set; }
        public DeviceSetupService()
        {
            CurrentDeviceHotspotNetwork = new WifiNetwork();
        }

        public static class DeviceTypeValues
        {
            public const string ButtonSensor = "Edison.Devices.ButtonSensor";
            public const string SmartBulb = "Edison.Devices.SmartBulb";
            public const string SoundSensor = "Edison.Devices.SoundSensor";
        }

        public DeviceModel CurrentDeviceModel { get; set; } = new DeviceModel();

        public string OriginalSSID { get; set; }

        public WifiNetwork CurrentWifiNetwork { get; set; }
        public WifiNetwork CurrentDeviceHotspotNetwork { get; set; }

        public string DefaultPassword => "Edison1234";//"Edison1234";
        public string DefaultPortalPassword => "Edison1234";

        public string ConnectedWifiSSID = "";

        public string WiFiPassword { get; set; }
        public string PortalPassword { get; set; }

        public string DeviceTypeAsString => CurrentDeviceModel?.DeviceType ?? "";
        public string DeviceTypeAsFriendlyString => DeviceTypeToFriendlyString(DeviceTypeAsString);

        public void ClearDevice()
        {
            CurrentDeviceModel = new DeviceModel();
            CurrentDeviceHotspotNetwork = new WifiNetwork();
        }
         
        public static bool SSIDIsEdisonDevice(string ssid) => ssid.Contains("EDISON_"); // android has quotes around it so starts with won't work

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
            if (CurrentDeviceModel.Custom == null)
            {
                CurrentDeviceModel.Custom = new Dictionary<string, object>();
            }

            CurrentDeviceModel.Custom[key] = value;
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
    }
}