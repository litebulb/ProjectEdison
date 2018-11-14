using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class EventClusterCloseModel
    {
        public Guid EventClusterId { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime ClosureDate { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime EndDate { get; set; }
    }
}
