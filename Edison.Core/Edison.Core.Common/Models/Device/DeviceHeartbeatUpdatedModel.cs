using Newtonsoft.Json;
using System;

namespace Edison.Core.Common.Models
{
    public class DeviceHeartbeatUpdatedModel
    {
        public DeviceModel Device { get; set; }
        public bool NeedsUpdate { get; set; }      
    }
}
