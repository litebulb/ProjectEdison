using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class DeviceCreationModel
    {
        public Guid DeviceId { get; set; }
        public string DeviceType { get; set; }
        public bool Sensor { get; set; }
        public Geolocation Geolocation { get; set; }
        public string Name { get; set; }
        public string Location1 { get; set; }
        public string Location2 { get; set; }
        public string Location3 { get; set; }
        public Dictionary<string, object> Custom { get; set; }
        public Dictionary<string, object> Desired { get; set; }
    }
}
