using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class DeviceMobileModel
    {
        public DeviceMobileModel()
        {
            Enabled = true;
            Custom = new Dictionary<string, object>();
        }
        public Guid DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public string Platform { get { return Custom["Platform"] as string; } set { Custom["Platform"] = value as string; } }
        public string MobileId { get { return Custom["MobileId"] as string; } set { Custom["MobileId"] = value as string; } }
        public string RegistrationId { get { return Custom["RegistrationId"] as string; } set { Custom["RegistrationId"] = value as string; } }
        public string Email { get { return Custom["Email"] as string; } set { Custom["Email"] = value as string; } }
        public Dictionary<string, object> Custom { get; set; }
    }
}
