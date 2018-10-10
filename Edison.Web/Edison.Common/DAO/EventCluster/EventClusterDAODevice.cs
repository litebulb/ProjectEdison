using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Edison.Common.DAO
{
    public class EventClusterDAODevice
    {
        public Guid DeviceId { get; set; }
        public string DeviceType { get; set; }
        public bool Online { get; set; }
        public string LocationName { get; set; }
        public string LocationLevel1 { get; set; }
        public string LocationLevel2 { get; set; }
        public string LocationLevel3 { get; set; }
        public GeolocationDAOObject Geolocation { get; set; }

    }
}
