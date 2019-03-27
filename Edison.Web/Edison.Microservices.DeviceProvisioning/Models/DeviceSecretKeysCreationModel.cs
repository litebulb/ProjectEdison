using System;

namespace Edison.DeviceProvisioning.Models
{
    public class DeviceSecretKeysCreationModel
    {
        public Guid DeviceId { get; set; }
        public string SSIDName { get; set; }
    }
}
