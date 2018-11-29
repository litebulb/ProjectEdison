using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class DevicesUpdateDesiredModel
    {
        public List<Guid> DeviceIds { get; set; }
        public Dictionary<string, object> Desired { get; set; }
        
    }
}
