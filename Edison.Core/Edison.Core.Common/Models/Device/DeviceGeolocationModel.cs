using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class DeviceGeolocationModel
    {
        public Geolocation ResponseEpicenterLocation { get; set; }
        public double Radius { get; set; }
        public bool FetchSensors { get; set; }
    }
}
