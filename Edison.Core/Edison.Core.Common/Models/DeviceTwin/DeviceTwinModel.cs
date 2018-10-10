using Edison.Core.Common.Models;
using System;

namespace Edison.Core.Common.Models
{
    public class DeviceTwinModel
    {
        public Guid DeviceId { get; set; }
        public DeviceTwinTagsModel Tags { get; set; }
        public DeviceTwinPropertiesModel Properties { get; set; }
    }

    
}
