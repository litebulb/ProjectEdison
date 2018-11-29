using Edison.Common.Interfaces;
using Edison.Core.Common;
using Newtonsoft.Json;
using System;

namespace Edison.Common.DAO
{
    /// <summary>
    /// DAO - Contains information pertaining to a set of events grouped together by device and a time constraint
    /// </summary>
    public class EventClusterDAO : IEntityDAO
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string EventType { get; set; }

        public EventClusterDeviceDAOObject Device { get; set; }

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
