using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class DevicesUpdateTagsModel
    {
        public List<Guid> DeviceIds { get; set; }
        public string DeviceType { get; set; }
        public bool Sensor { get; set; }
        public Geolocation Geolocation { get; set; }
        public string LocationName { get; set; }
        public string LocationLevel1 { get; set; }
        public string LocationLevel2 { get; set; }
        public string LocationLevel3 { get; set; }
        public Dictionary<string, object> Custom { get; set; }     
    }

    
}
