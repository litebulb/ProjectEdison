namespace Edison.Devices.Onboarding.Common.Models
{
    public class RequestCommandSetDeviceSecretKeys : RequestCommand
    {
        public string APSsid { get; set; }
        public string APPassword { get; set; }
        public string PortalPassword { get; set; }
        public string EncryptionKey { get; set; }
    }
}
