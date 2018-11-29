using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.Documents;
using AutoMapper;
using Edison.Core.Common.Models;
using Edison.Common.Interfaces;
using Edison.Common.DAO;

namespace Edison.Api.Helpers
{
    /// <summary>
    /// Manager for the Event Clusters repository
    /// </summary>
    public class EventClustersDataManager
    {
        private ICosmosDBRepository<EventClusterDAO> _repoEventClusters;
        private ICosmosDBRepository<DeviceDAO> _repoDevices;
        private IMapper _mapper;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public EventClustersDataManager(IMapper mapper,
            ICosmosDBRepository<EventClusterDAO> repoEventClusters,
            ICosmosDBRepository<DeviceDAO> repoDevices)
        {
            _mapper = mapper;
            _repoEventClusters = repoEventClusters;
            _repoDevices = repoDevices;
        }

        /// <summary>
        /// Get Event Cluster by Id
        /// </summary>
        /// <param name="eventClusterId">Event Cluster Id</param>
        /// <returns>EventClusterModel</returns>
        public async Task<EventClusterModel> GetEventCluster(Guid eventClusterId)
        {
            EventClusterDAO eventCluster = await _repoEventClusters.GetItemAsync(eventClusterId);
            return _mapper.Map<EventClusterModel>(eventCluster);
        }

        /// <summary>
        /// Get Event Clusters
        /// Only the last 3 events are returned
        /// </summary>
        /// <returns>List of Event Clusters</returns>
        public async Task<IEnumerable<EventClusterModel>> GetEventClusters()
        {
            IEnumerable<EventClusterDAO> eventClusters = await _repoEventClusters.GetItemsAsync(
                p => p.ClosureDate.Value == null || (p.EndDate.Value != null && p.EndDate > DateTime.UtcNow),
                p => new EventClusterDAO()
                {
                    Id = p.Id,
                    Events = new EventDAOObject[] {
                        p.Events[0],
                        p.Events[1],
                        p.Events[2]
                    }, //Used to force only the first 3 entries
                    EventCount = p.EventCount,
                    EventType = p.EventType,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    UpdateDate = p.UpdateDate,
                    ClosureDate = p.ClosureDate,
                    Device = p.Device
                }
                );

            var orderedEventClusters = eventClusters.OrderByDescending(p => p.StartDate);
            return _mapper.Map<IEnumerable<EventClusterModel>>(orderedEventClusters);
        }

        /// <summary>
        /// Get a list of Event Clusters in a specific geolocation radius
        /// </summary>
        /// <param name="eventClusterGeolocationObj">EventClusterGeolocationModel</param>
        /// <returns>List of Event Clusters Ids</returns>
        public async Task<IEnumerable<Guid>> GetClustersInRadius(EventClusterGeolocationModel eventClusterGeolocationObj)
        {
            if (eventClusterGeolocationObj == null || eventClusterGeolocationObj.ResponseGeolocationPointLocation == null)
                return new List<Guid>();

            IEnumerable<EventClusterDAO> eventClusters = await _repoEventClusters.GetItemsAsync(
               p => p.ClosureDate.Value == null,
               p => new EventClusterDAO()
               {
                   Id = p.Id,
                   EventType = p.EventType,
                   Device = new EventClusterDeviceDAOObject() { Geolocation = p.Device.Geolocation }
               }
               );

            List<Guid> output = new List<Guid>();
            GeolocationDAOObject daoGeocodeCenterPoint = _mapper.Map<GeolocationDAOObject>(eventClusterGeolocationObj.ResponseGeolocationPointLocation);
            foreach (EventClusterDAO eventClusterObj in eventClusters)
                if (RadiusHelper.IsWithinRadius(eventClusterObj.Device.Geolocation, daoGeocodeCenterPoint, eventClusterGeolocationObj.Radius))
                    output.Add(new Guid(eventClusterObj.Id));
            return output;
        }

        /// <summary>
        /// Create or Update an Event Cluster
        /// </summary>
        /// <param name="eventObj">EventClusterCreationModel</param>
        /// <returns>EventClusterModel</returns>
        public async Task<EventClusterModel> CreateOrUpdateEventCluster(EventClusterCreationModel eventObj)
        {
            //Look for existing cluster that matches DeviceId + EventType and hasn't ended yet.
            EventClusterDAO eventCluster = await _repoEventClusters.GetItemAsync(eventObj.EventClusterId);

            //Create
            if (eventCluster == null)
                return await CreateEventCluster(eventObj);

            eventCluster.Events = eventCluster.Events.Append(_mapper.Map<EventDAOObject>(eventObj)).OrderByDescending(p => p.Date).ToArray();
            eventCluster.EventCount++;
            try
            {
                await _repoEventClusters.UpdateItemAsync(eventCluster);
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    return await CreateOrUpdateEventCluster(eventObj);
                throw e;
            }

            var output = _mapper.Map<EventClusterModel>(eventCluster);
            output.Events = output.Events.Take(3);
            return output;
        }

        /// <summary>
        /// Create an Event Cluster
        /// </summary>
        /// <param name="eventObj">EventClusterCreationModel</param>
        /// <returns>EventClusterModel</returns>
        public async Task<EventClusterModel> CreateEventCluster(EventClusterCreationModel eventObj)
        {
            //If device doesn't exist, throw exception
            DeviceDAO deviceEntity = await _repoDevices.GetItemAsync(eventObj.DeviceId);
            if (deviceEntity == null)
                throw new Exception($"No device found that matches DeviceId: {eventObj.DeviceId}");

            EventClusterDAO eventCluster = new EventClusterDAO()
            {
                Id = eventObj.EventClusterId.ToString(),
                Device = _mapper.Map<EventClusterDeviceDAOObject>(deviceEntity),
                EventType = eventObj.EventType.ToLower(),
                EventCount = 1,
                Events = new EventDAOObject[] { _mapper.Map<EventDAOObject>(eventObj) },
                StartDate = eventObj.Date
            };
            eventCluster.Id = await _repoEventClusters.CreateItemAsync(eventCluster);
            if (_repoEventClusters.IsDocumentKeyNull(eventCluster))
                throw new Exception($"An error occured when creating a new cluster id for DeviceId: {eventObj.DeviceId}");

            return _mapper.Map<EventClusterModel>(eventCluster);
        }

        /// <summary>
        /// Update the geolocation of a mobile device
        /// </summary>
        /// <param name="geolocation">Geolocation of the event cluster</param>
        /// <param name="deviceId">Device Id</param>
        /// <returnsEventClusterGeolocationUpdateResultModel></returns>
        public async Task<EventClusterGeolocationUpdateResultModel> UpdateGeolocation(Geolocation geolocation, Guid deviceId)
        {
            if (geolocation == null)
                throw new Exception($"No Geolocation found: {geolocation}");
            if (deviceId == Guid.Empty || deviceId == null)
                throw new Exception($"DeviceId not found");

            EventClusterDAO eventClusterDAO = await _repoEventClusters.GetItemAsync(p => p.Device.DeviceId == deviceId && p.ClosureDate.Value == null);

            if (eventClusterDAO != null)
            {
                if (geolocation.Latitude == eventClusterDAO.Device.Geolocation.Latitude &&
                    geolocation.Longitude == eventClusterDAO.Device.Geolocation.Longitude)
                    return new EventClusterGeolocationUpdateResultModel() { Success = false };

                string etag = eventClusterDAO.ETag;
                eventClusterDAO.Device.Geolocation = _mapper.Map<GeolocationDAOObject>(geolocation);

                try
                {
                    var result = await _repoEventClusters.UpdateItemAsync(eventClusterDAO);
                    if (result)
                    {
                        return new EventClusterGeolocationUpdateResultModel()
                        {
                            Success = true,
                            EventCluster = _mapper.Map<EventClusterModel>(eventClusterDAO)
                        };
                    }
                    throw new Exception($"Error while updating device geolocation: {deviceId}.");

                }
                catch (DocumentClientException e)
                {
                    //Update concurrency issue, retrying
                    if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                        return await UpdateGeolocation(geolocation, deviceId);
                    throw e;
                }
            }
            return new EventClusterGeolocationUpdateResultModel() { Success = false };
        }

        /// <summary>
        /// Close an Event Cluster
        /// </summary>
        /// <param name="eventObj">EventClusterCloseModel</param>
        /// <returns>EventClusterModel</returns>
        public async Task<EventClusterModel> CloseEventCluster(EventClusterCloseModel eventObj)
        {
            EventClusterDAO eventCluster = await _repoEventClusters.GetItemAsync(eventObj.EventClusterId);
            eventCluster.ClosureDate = eventObj.ClosureDate;
            eventCluster.EndDate = eventObj.EndDate;

            try
            {
                await _repoEventClusters.UpdateItemAsync(eventCluster);
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    await CloseEventCluster(eventObj);
                throw e;
            }

            var output = _mapper.Map<EventClusterModel>(eventCluster);
            output.Events = output.Events.Take(3);
            return output;
        }
    }
}
