using Edison.Devices.Common;

namespace Edison.Devices.SoundSensor
{
    internal class SoundSensorState : IDeviceState
    {
        public DeviceState State { get; set; }
        public double DecibelThreshold { get; set; }
        public int SoundTriggerDelay { get; set; }
        public SoundSensorGpioConfig GpioConfig { get; set; }
    }
}
