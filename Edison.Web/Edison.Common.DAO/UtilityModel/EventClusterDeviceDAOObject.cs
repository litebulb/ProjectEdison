using System;

namespace Edison.Common.DAO
{
    public class EventClusterDeviceDAOObject
    {
        public Guid DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string Name { get; set; }
        public string Location1 { get; set; }
        public string Location2 { get; set; }
        public string Location3 { get; set; }
        public GeolocationDAOObject Geolocation { get; set; }

    }
}
