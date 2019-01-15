using System;
namespace Edison.Mobile.Admin.Client.Core.Models
{
    public class RequestCommandProvisionDevice : RequestCommand
    {
        public DeviceCertificateModel DeviceCertificateInformation { get; set; }
    }
}
