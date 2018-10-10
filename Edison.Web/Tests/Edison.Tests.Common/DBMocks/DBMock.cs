using AutoMapper;
using Edison.Common.DAO;
using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Edison.Tests
{
    public static class DBMock
    {
        private static List<DeviceDAO> _DBDevices;
        private static List<EventClusterDAO> _DBEventClusters;
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
            _DBDevices = DBMockDevice.DBDevices;
            _DBEventClusters = DBMockEventCluster.DBEventClusters;
        }

        public static EventClusterModel GetEventCluster(Guid eventClusterId)
        {
            EventClusterDAO eventCluster = _DBEventClusters.Find(p => p.Id == eventClusterId);
            return Mapper.Map<EventClusterModel>(eventCluster);
        }

        public static IEnumerable<EventClusterModel> GetEventClusters()
        {
            IEnumerable<EventClusterDAO> eventClusters = _DBEventClusters.FindAll(p => p.ClosureDate == DateTime.MinValue || p.EndDate > DateTime.UtcNow);

            //Only keep the first eventCluster for each device + event type sorted by start date descending
            var groupedEventClusters =
                eventClusters
                .OrderByDescending(p => p.StartDate)
                .GroupBy(x => new { x.Device.DeviceId, x.EventType })
                .Select(p => p.First());

            return Mapper.Map<IEnumerable<EventClusterModel>>(groupedEventClusters);
        }

        public static EventClusterModel CreateOrUpdateEventCluster(EventClusterCreationModel eventObj)
        {
            //If device doesn't exist, throw exception
            DeviceDAO deviceEntity = _DBDevices.Find(p => p.Id == eventObj.DeviceId);
            if (deviceEntity == null)
                throw new Exception($"No device found that matches DeviceId: {eventObj.DeviceId}");

            EventClusterDAO eventCluster = new EventClusterDAO()
            {
                Id = eventObj.EventClusterId,
                Device = Mapper.Map<EventClusterDAODevice>(deviceEntity),
                EventType = eventObj.EventType.ToLower(),
                EventCount = 1,
                Events = new EventDAOObject[] { Mapper.Map<EventDAOObject>(eventObj) },
                StartDate = eventObj.Date
            };
            _DBEventClusters.Add(eventCluster);
            if (eventCluster.Id == Guid.Empty)
                throw new Exception($"An error occured when creating a new cluster id for DeviceId: {eventObj.DeviceId}");

            return Mapper.Map<EventClusterModel>(eventCluster);
        }

        public static EventClusterModel AddEventToCluster(EventClusterUpdateModel eventObj)
        {
            EventClusterDAO eventCluster = _DBEventClusters.Find(p => p.Id == eventObj.EventClusterId);
            if (eventCluster == null)
                throw new Exception($"No eventCluster found that matches EventClusterId: {eventObj.EventClusterId}");

            eventCluster.Events = eventCluster.Events.Prepend(Mapper.Map<EventDAOObject>(eventObj)).ToArray();
            eventCluster.EventCount++;

            var output = Mapper.Map<EventClusterModel>(eventCluster);
            output.Events = output.Events.Take(3);
            return output;
        }

        public static EventClusterModel CloseEventCluster(EventClusterCloseModel eventObj)
        {
            EventClusterDAO eventCluster = _DBEventClusters.Find(p => p.Id == eventObj.EventClusterId);
            eventCluster.ClosureDate = eventObj.ClosureDate;
            eventCluster.EndDate = eventObj.EndDate;

            var output = Mapper.Map<EventClusterModel>(eventCluster);
            output.Events = output.Events.Take(3);
            return output;
        }
    }
}
