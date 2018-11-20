using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Devices.Onboarding.Client.Models
{
    public class DeviceSecretKeysModel
    {
        public string AccessPointPassword { get; set; }
        public string PortalPassword { get; set; }
        public string EncryptionKey { get; set; }
    }
}
