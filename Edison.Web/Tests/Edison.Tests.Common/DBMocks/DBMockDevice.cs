using Edison.Common.DAO;
using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Edison.Tests
{
    public static class DBMockDevice
    {
        public static List<DeviceDAO> DBDevices;

        public static void Init()
        {
            CreateDeviceDAO("fbc64b5c-ff21-4ade-9440-85f7b16ef01e", "ButtonSensor", 41.8855050, -87.6248890);
            CreateDeviceDAO("7776a948-90f8-4ffd-9578-f8078b07d96f", "ButtonSensor", 41.8855190, -87.6265480);
            CreateDeviceDAO("c337f50b-134a-4d83-8f40-18f6691e4dbb", "ButtonSensor", 41.8855120, -87.6265430);
            CreateDeviceDAO("f771c2e7-96c2-450b-912e-262588bdeeaa", "SoundSensor", 41.8855050, -87.6248890);
            CreateDeviceDAO("d9ae43c1-aaa7-401c-98c7-efbc5e58aa1c", "SmartBulb", 41.8855150, -87.6248790);
        }

        private static void CreateDeviceDAO(string id, string type, double latitude, double longitude, bool enabled = true)
        {
            if (DBDevices == null)
                DBDevices = new List<DeviceDAO>();

            int index = DBDevices.Count + 1;
            DBDevices.Add(new DeviceDAO()
            {
                Id = id,
                CreationDate = DateTime.UtcNow.AddMinutes(-100),
                DeviceType = type,
                ETag = Guid.NewGuid().ToString(),
                Geolocation = new GeolocationDAOObject()
                {
                    Latitude = latitude,
                    Longitude = longitude
                },
                UpdateDate = DateTime.UtcNow.AddMinutes(-100),
                IoTDevice = true,
                Sensor = true,
                Enabled = enabled,
                Name = type + index,
                Location1 = $"{type}{index} Location1",
                Location2 = $"{type}{index} Location2",
                Location3 = $"{type}{index} Location3"
            });
        }
    }
}
