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
            DBDevices = new List<DeviceDAO>()
            {
                new DeviceDAO()
                {
                    Id = new Guid("fbc64b5c-ff21-4ade-9440-85f7b16ef01e"),
                    CreationDate = DateTime.UtcNow.AddMinutes(-100),
                    DeviceType = "button",
                    ETag = Guid.NewGuid().ToString(),
                    Geolocation = new GeolocationDAOObject()
                    {
                        Latitude = 41.8855050,
                        Longitude = -87.6248890
                    },
                    UpdateDate = DateTime.UtcNow.AddMinutes(-100),
                    LocationLevel1 = "BlueMetal Office",
                    LocationLevel2 = "Floor 11",
                    LocationLevel3 = "Room A"
                },
                new DeviceDAO()
                {
                    Id = new Guid("7776a948-90f8-4ffd-9578-f8078b07d96f"),
                    CreationDate = DateTime.UtcNow.AddMinutes(-100),
                    DeviceType = "button",
                    ETag = Guid.NewGuid().ToString(),
                    Geolocation = new GeolocationDAOObject()
                    {
                        Latitude = 41.8855190,
                        Longitude = -87.6265480
                    },
                    UpdateDate = DateTime.UtcNow.AddMinutes(-100),
                    LocationLevel1 = "Office Space",
                    LocationLevel2 = "Floor 10",
                    LocationLevel3 = "Room C"
                },new DeviceDAO()
                {
                    Id = new Guid("c337f50b-134a-4d83-8f40-18f6691e4dbb"),
                    CreationDate = DateTime.UtcNow.AddMinutes(-100),
                    DeviceType = "button",
                    ETag = Guid.NewGuid().ToString(),
                    Geolocation = new GeolocationDAOObject()
                    {
                        Latitude = 41.8855190,
                        Longitude = -87.6265480
                    },
                    UpdateDate = DateTime.UtcNow.AddMinutes(-100),
                    LocationLevel1 = "Office Space",
                    LocationLevel2 = "Floor 10",
                    LocationLevel3 = "Room C"
                },
                new DeviceDAO()
                {
                    Id = new Guid("f771c2e7-96c2-450b-912e-262588bdeeaa"),
                    CreationDate = DateTime.UtcNow.AddMinutes(-100),
                    DeviceType = "soundsensor",
                    ETag = Guid.NewGuid().ToString(),
                    Geolocation = new GeolocationDAOObject()
                    {
                        Latitude = 41.8855050,
                        Longitude = -87.6248890
                    },
                    UpdateDate = DateTime.UtcNow.AddMinutes(-100),
                    LocationLevel1 = "BlueMetal Office",
                    LocationLevel2 = "Floor 11",
                    LocationLevel3 = "Room A"
                }
            };

        }
    }
}
