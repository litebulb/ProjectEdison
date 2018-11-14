using Edison.Common.DAO;
using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Edison.Tests
{
    public static class DBMockEventCluster
    {
        public static List<EventClusterDAO> DBEventClusters;

        public static void Init()
        {
            DBEventClusters = new List<EventClusterDAO>
            {
                new EventClusterDAO()
                {
                    Device = GetEventClusterDAOSource("fbc64b5c-ff21-4ade-9440-85f7b16ef01e"),
                    CreationDate = DateTime.UtcNow.AddMinutes(-10),
                    StartDate = DateTime.UtcNow.AddMinutes(-10),
                    EndDate = DateTime.MinValue,
                    ClosureDate = DateTime.MinValue,
                    EventCount = 2,
                    Events = new EventDAOObject[]
                    {
                        new EventDAOObject()
                        {
                             Date = DateTime.UtcNow.AddMinutes(-10),
                             Metadata = null
                        },
                        new EventDAOObject()
                        {
                             Date = DateTime.UtcNow.AddMinutes(-5),
                             Metadata = null
                        }
                    },
                    EventType = "button",
                    UpdateDate = DateTime.UtcNow.AddMinutes(-5),
                    ETag = Guid.NewGuid().ToString(),
                    Id = "fa2528dc-bdd4-4e27-bac6-56b8f38024e8"
                },
                new EventClusterDAO()
                {
                    Device = GetEventClusterDAOSource("fbc64b5c-ff21-4ade-9440-85f7b16ef01e"),
                    CreationDate = DateTime.UtcNow.AddMinutes(-90),
                    StartDate = DateTime.UtcNow.AddMinutes(-90),
                    EndDate = DateTime.UtcNow.AddMinutes(-75),
                    ClosureDate = DateTime.UtcNow.AddMinutes(-15),
                    EventCount = 1,
                    Events = new EventDAOObject[]
                    {
                        new EventDAOObject()
                        {
                             Date = DateTime.UtcNow.AddMinutes(-10),
                             Metadata = null
                        }
                    },
                    EventType = "button",
                    UpdateDate = DateTime.UtcNow.AddMinutes(-75),
                    ETag = Guid.NewGuid().ToString(),
                    Id = "9c7a02f5-b657-4b53-a11f-b5fc8c98194c",
                },
                new EventClusterDAO()
                {
                    Device = GetEventClusterDAOSource("7776a948-90f8-4ffd-9578-f8078b07d96f"),
                    CreationDate = DateTime.UtcNow.AddMinutes(-60),
                    StartDate = DateTime.UtcNow.AddMinutes(-60),
                    EndDate = DateTime.UtcNow.AddMinutes(-5),
                    ClosureDate = DateTime.MinValue,
                    EventCount = 5,
                    Events = new EventDAOObject[]
                    {
                        new EventDAOObject()
                        {
                             Date = DateTime.UtcNow.AddMinutes(-60),
                             Metadata = null
                        },
                        new EventDAOObject()
                        {
                             Date = DateTime.UtcNow.AddMinutes(-40),
                             Metadata = null
                        },
                        new EventDAOObject()
                        {
                             Date = DateTime.UtcNow.AddMinutes(-30),
                             Metadata = null
                        },
                        new EventDAOObject()
                        {
                             Date = DateTime.UtcNow.AddMinutes(-25),
                             Metadata = null
                        },
                        new EventDAOObject()
                        {
                             Date = DateTime.UtcNow.AddMinutes(-20),
                             Metadata = null
                        }
                    },
                    EventType = "button",
                    UpdateDate = DateTime.UtcNow.AddMinutes(-5),
                    ETag = Guid.NewGuid().ToString(),
                    Id = "ddd6db1a-0a6f-447c-a0ed-dca531c5ab11",
                },
                new EventClusterDAO()
                {
                    Device = GetEventClusterDAOSource("f771c2e7-96c2-450b-912e-262588bdeeaa"),
                    CreationDate = DateTime.UtcNow.AddMinutes(-5),
                    StartDate = DateTime.UtcNow.AddMinutes(-5),
                    EndDate = DateTime.MinValue,
                    ClosureDate = DateTime.MinValue,
                    EventCount = 1,
                    Events = new EventDAOObject[]
                    {
                        new EventDAOObject()
                        {
                             Date = DateTime.UtcNow.AddMinutes(-60),
                             Metadata = new Dictionary<string, object>() { { "decibel", 123.8 } }
                        }
                    },
                    EventType = "sound",
                    UpdateDate = DateTime.UtcNow.AddMinutes(-5),
                    ETag = Guid.NewGuid().ToString(),
                    Id = "539b392a-7ad0-4a91-86ad-bdf5e8056d16",
                }
            };
        }

        private static EventClusterDeviceDAOObject GetEventClusterDAOSource(string deviceId)
        {
            return AutoMapper.Mapper.Map<EventClusterDeviceDAOObject>(DBMockDevice.DBDevices.Find(p => p.Id == deviceId));
        }
    }
}
