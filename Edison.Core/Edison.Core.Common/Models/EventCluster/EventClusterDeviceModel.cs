using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class EventClusterDeviceModel
    {
        public Guid DeviceId { get; set; }
        public string DeviceType { get; set; }
        public bool Online { get; set; }
        public string LocationName { get; set; }
        public string LocationLevel1 { get; set; }
        public string LocationLevel2 { get; set; }
        public string LocationLevel3 { get; set; }
        public Geolocation Geolocation { get; set; }
    }
}
