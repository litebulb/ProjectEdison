using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class EventClusterCreationModel
    {
        public Guid EventClusterId { get; set; }
        public Guid DeviceId { get; set; }
        public string EventType { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string,object> Metadata { get; set; }
    }
}
