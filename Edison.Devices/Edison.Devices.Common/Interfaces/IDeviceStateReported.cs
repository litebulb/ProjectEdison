namespace Edison.Devices.Common
{
    public interface IDeviceStateReported
    {
        string FirmwareVersion { get; }
        IDeviceState DeviceState { get; set; }
    }
}
