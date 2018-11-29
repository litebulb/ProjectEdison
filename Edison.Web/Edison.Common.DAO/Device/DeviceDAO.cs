using Edison.Common.Interfaces;
using Edison.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Edison.Common.DAO
{
    /// <summary>
    /// DAO - Contains information pertaining to a IoT Device or a Phone
    /// </summary>
    public class DeviceDAO : IEntityDAO
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string DeviceType { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime LastAccessTime { get; set; }
        public bool Sensor { get; set; }
        public bool IoTDevice { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string Location1 { get; set; }
        public string Location2 { get; set; }
        public string Location3 { get; set; }
        public GeolocationDAOObject Geolocation { get; set; }
        public DateTime CreationDate { get; set; }       
        public DateTime UpdateDate { get; set; }
        public Dictionary<string, object> Custom { get; set; }
        public Dictionary<string, object> Reported { get; set; }
        public Dictionary<string, object> Desired { get; set; }
        public string ETag { get; set; }
        
    }
}
