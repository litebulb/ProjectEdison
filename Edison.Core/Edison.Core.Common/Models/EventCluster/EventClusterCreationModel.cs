using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class EventClusterCreationModel
    {
        public Guid EventClusterId { get; set; }
        public Guid DeviceId { get; set; }
        //public EventSourceModel SourceData { get; set; }
        public string EventType { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Date { get; set; }
        public Dictionary<string,object> Metadata { get; set; }
    }
}
