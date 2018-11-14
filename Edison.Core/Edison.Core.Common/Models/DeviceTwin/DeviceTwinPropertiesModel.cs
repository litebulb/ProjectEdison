using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class DeviceTwinPropertiesModel
    {
        public Dictionary<string, object> Reported { get; set; }
        public Dictionary<string, object> Desired { get; set; }
    }
}
