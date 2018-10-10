using Newtonsoft.Json;
using System;

namespace Edison.Core.Common.Models
{
    public class DeviceUpdateHeartbeatModel
    {
        public Guid DeviceId { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime LastAccessTime { get; set; }      
    }
}
