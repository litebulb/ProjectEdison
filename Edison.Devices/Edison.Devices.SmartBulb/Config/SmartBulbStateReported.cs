using Edison.Devices.Common;

namespace Edison.Devices.SmartBulb
{
    internal class SmartBulbStateReported : IDeviceStateReported
    {
        public string FirmwareVersion { get { return "1.0"; } }
        public IDeviceState DeviceState { get; set; }
    }
}
