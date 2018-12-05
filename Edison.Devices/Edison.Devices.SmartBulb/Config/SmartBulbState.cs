using Edison.Devices.Common;

namespace Edison.Devices.SmartBulb
{
    internal class SmartBulbState : IDeviceState
    {
        public DeviceState State { get; set; }
        public LightState LightState { get; set; }
        public ColorState Color { get; set; }
        public int FlashFrequency { get; set; }
        public bool IgnoreFlashAlerts { get; set; }
        public int Brightness { get; set; }
        public SmartBulbGpioConfig GpioConfig { get; set; }
    }
}
