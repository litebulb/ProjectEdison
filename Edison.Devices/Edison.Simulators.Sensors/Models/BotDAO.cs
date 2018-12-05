using Edison.Common.Interfaces;
using Newtonsoft.Json;
using System;

namespace Edison.Simulators.Sensors.Models
{
    //Workaround for Bot collection
    public class BotDAO : IEntityDAO
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string ETag { get; set; }
    }
}
