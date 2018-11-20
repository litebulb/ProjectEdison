using Edison.DeviceProvisioning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.DeviceProvisioning.Config
{
    public class DeviceProvisioningOptions
    {
        public string DeviceCertificatePassword { get; set; }
        public int DeviceCertificateValidityDays { get; set; }
        public List<DeviceProvisioningCertificateEntry> SigningCertificates { get; set; }
        public string DPSInstance { get; set; }
        public string DPSIdScope { get; set; }
        public string KeyVaultAddress { get; set; }
        public DeviceSecretKeysModel DefaultSecrets { get; set; }
    }
}
