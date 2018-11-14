using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class EventClusterUpdateModel
    {
        public Guid EventClusterId { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Date { get; set; }
        public IDictionary<string, object> Metadata { get; set; }
    }
}
