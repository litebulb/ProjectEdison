using AutoMapper;
using Edison.Common.DAO;
using Edison.Common.Interfaces;
using Edison.Core.Common.Models;
using Edison.Tests.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Edison.Tests
{
    public static class DBMock
    {
        private static ICosmosDBRepository<DeviceDAO> _DBDevices;
        //private static ICosmosDBRepository<ActionPlanDAO> _DBActionPlans;
        private static ICosmosDBRepository<EventClusterDAO> _DBEventClusters;
        //private static ICosmosDBRepository<ResponseDAO> _DBResponses;
        //private static ICosmosDBRepository<NotificationDAO> _DBNotifications;
        //private static ICosmosDBRepository<ChatReportDAO> _ChatReportDAO;
        private static bool MapperInitialized = false;

        public static void Init()
        {
            if (!MapperInitialized)
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                });
                Mapper.AssertConfigurationIsValid();
                MapperInitialized = true;
            }

            DBMockDevice.Init();
            DBMockEventCluster.Init();
            _DBDevices = new InMemoryDBRepository<DeviceDAO>(DBMockDevice.DBDevices, LoggerHelper.CreateLogger< InMemoryDBRepository<DeviceDAO>>().Object);
            _DBEventClusters = new InMemoryDBRepository<EventClusterDAO>(DBMockEventCluster.DBEventClusters, LoggerHelper.CreateLogger<InMemoryDBRepository<EventClusterDAO>>().Object);
        }
    }
}
