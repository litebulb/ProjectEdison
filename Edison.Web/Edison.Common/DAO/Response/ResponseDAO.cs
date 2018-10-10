using Edison.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Edison.Common.DAO
{
    public class ResponseDAO : IEntityDAO
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        public int ResponseState { get; set; }
        public Guid ResponderUserId { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime? EndDate { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime CreationDate { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime UpdateDate { get; set; }
        public ActionPlanDAOObject ActionPlan { get; set; }
        public Guid PrimaryEventClusterId { get; set; }
        public GeolocationDAOObject Geolocation { get; set; }
        public IEnumerable<Guid> EventClusterIds { get; set; }
        public string ETag { get; set; }
        
    }
}
