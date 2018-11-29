using System;

namespace Edison.Core.Common.Models
{
    public class EventClusterDeviceModel
    {
        public Guid DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string Name { get; set; }
        public string Location1 { get; set; }
        public string Location2 { get; set; }
        public string Location3 { get; set; }
        public Geolocation Geolocation { get; set; }
    }
}
