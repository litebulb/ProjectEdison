using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class EventModel
    {
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Date { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}
