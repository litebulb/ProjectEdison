using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class DevicesLaunchDirectMethodModel
    {
        public List<Guid> DeviceIds { get; set; }
        public string MethodName { get; set; }
        public string Payload { get; set; }
    }
}
