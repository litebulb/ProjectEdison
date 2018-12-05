using Edison.Devices.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Devices.SmartBulb
{
    internal class SmartBulbState : IDeviceState
    {
        public DeviceState State { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LightState LightState { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ColorState Color { get; set; }
        public int FlashFrequency { get; set; }
        public bool IgnoreFlashAlerts { get; set; }
        public int Brightness { get; set; }
        public SmartBulbGpioConfig GpioConfig { get; set; }
    }
}
