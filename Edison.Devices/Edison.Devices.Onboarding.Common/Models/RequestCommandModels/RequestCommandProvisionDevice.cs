namespace Edison.Devices.Onboarding.Common.Models
{
    public class RequestCommandProvisionDevice : RequestCommand
    {
        public DeviceCertificateModel DeviceCertificateInformation { get; set; }
    }
}
