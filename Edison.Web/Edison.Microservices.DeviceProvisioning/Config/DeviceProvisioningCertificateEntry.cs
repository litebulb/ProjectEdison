using Edison.Core.Common.Models;
using Edison.DeviceProvisioning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.DeviceProvisioning.Config
{
    public class DeviceProvisioningCertificateEntry
    {
        public string DeviceType { get; set; }
        public string Path { get; set; }
        public string Password { get; set; }
    }
}
