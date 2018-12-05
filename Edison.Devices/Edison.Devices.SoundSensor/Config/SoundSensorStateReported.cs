using Edison.Devices.Common;

namespace Edison.Devices.SoundSensor
{
    internal class SoundSensorStateReported : IDeviceStateReported
    {
        public string FirmwareVersion { get; set; }
        public IDeviceState DeviceState { get; set; }
    }
}
