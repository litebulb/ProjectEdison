using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class DeviceMapModel
    {
        public Guid DeviceId { get; set; }
        public string DeviceType { get; set; }
        public bool Online { get; set; }
        public Geolocation Geolocation { get; set; }     
    }
}
