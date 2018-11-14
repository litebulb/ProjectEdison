using Edison.Common.DAO;
using Edison.Common.Interfaces;
using Edison.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Simulators.Sensors.Models
{
    public class Entity : IEntityDAO
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime CreationDate { get; set; }

        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime UpdateDate { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        public string ETag { get; set; }
    }
}
