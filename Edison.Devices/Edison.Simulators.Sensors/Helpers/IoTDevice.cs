using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace Edison.Simulators.Sensors.Models.Helpers
{
    public class IoTDevice
    {
        [JsonIgnore]
        public DeviceClient Client { get; set; }

        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string Name { get; set; }
        public string Location1 { get; set; }
        public string Location2 { get; set; }
        public string Location3 { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public Dictionary<string, object> Desired { get; set; }
        public bool Sensor { get; set; }
        public bool Demo { get; set; }
        public bool Enabled { get; set; }
    }

    public class IoTDevicetMap : ClassMap<IoTDevice>
    {
        public IoTDevicetMap()
        {
            Map(m => m.Client).Ignore();
            Map(m => m.Desired).Ignore();
            Map(m => m.DeviceId);
            Map(m => m.DeviceType);
            Map(m => m.Name);
            Map(m => m.Location1);
            Map(m => m.Location2);
            Map(m => m.Location3);
            Map(m => m.Longitude);
            Map(m => m.Latitude);
            Map(m => m.Sensor);
            Map(m => m.Demo);
            Map(m => m.Enabled);
        }
    }
}