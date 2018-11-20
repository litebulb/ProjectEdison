using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.DeviceProvisioning.Models
{
    public class DeviceSecretKeysModel
    {
        public string AccessPointPassword { get; set; }
        public string PortalPassword { get; set; }
        public string EncryptionKey { get; set; }
    }
}
