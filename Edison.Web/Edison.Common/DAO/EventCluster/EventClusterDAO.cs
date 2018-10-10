using Edison.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Edison.Common.DAO
{
    public class EventClusterDAO : IEntityDAO
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        public string EventType { get; set; }

        public EventClusterDAODevice Device { get; set; }

        public int EventCount { get; set; }

        public EventDAOObject[] Events { get; set; }

        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime StartDate { get; set; }

        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime? ClosureDate { get; set; }

        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime? EndDate { get; set; }

        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime CreationDate { get; set; }

        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime UpdateDate { get; set; }

        public string ETag { get; set; }
        
    }
}
