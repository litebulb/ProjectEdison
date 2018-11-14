using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.SmartBulb
{
    public enum State
    {
        Unknown = 0,
        Off = 1,
        On = 2,
        Flashing = 3
    }

    public enum Color
    {
        Unknown = 0,
        Off = 1,
        White = 2,
        Red = 3,
        Green = 4,
        Blue = 5,
        Yellow = 6,
        Purple = 7,
        Cyan = 8
    }

    public sealed class SmartBulbConfig
    {
        public string FirmwareVersion { get; set; }
        public State State { get; set; }
        public int FlashFrequency { get; set; }
        public bool IgnoreFlashAlerts { get; set; }
        public int Brightness { get; set; }
        public SmartBulbGpioConfig GpioConfig { get; set; }
        public Color Color { get; set; }
    }

    public sealed class SmartBulbGpioConfig
    {
        public int GpioColorRed { get; set; }
        public int GpioColorGreen { get; set; }
        public int GpioColorBlue { get; set; }
    }
}
