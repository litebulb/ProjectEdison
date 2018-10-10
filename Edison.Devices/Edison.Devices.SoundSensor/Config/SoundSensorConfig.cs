using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.SoundSensor
{
    public enum State
    {
        Unknown = 0,
        Off = 1,
        On = 2
    }

    public sealed class SoundSensorConfig
    {
        public string FirmwareVersion { get; set; }
        public State State { get; set; }
        public double DecibelThreshold { get; set; }
        public SoundSensorGpioConfig GpioConfig { get; set; }
    }

    public sealed class SoundSensorGpioConfig
    {
        public int GpioSound { get; set; }
    }
}
