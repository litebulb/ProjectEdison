using Edison.Api.Config;
using Edison.Core.Common.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using System;
using Microsoft.Azure.Documents;
using System.Net;
using Edison.Common.Interfaces;
using Edison.Common.DAO;

namespace Edison.Api.Helpers
{
    public class EventClustersDataManager
    {
        private ICosmosDBRepository<EventClusterDAO> _repoEventClusters;
        private ICosmosDBRepository<DeviceDAO> _repoDevices;
        private IMapper _mapper;

        public EventClustersDataManager(IMapper mapper,
            ICosmosDBRepository<EventClusterDAO> repoEventClusters,
            ICosmosDBRepository<DeviceDAO> repoDevices)
        {
            _mapper = mapper;
            _repoEventClusters = repoEventClusters;
            _repoDevices = repoDevices;
        }

        public async Task<EventClusterModel> GetEventCluster(Guid eventClusterId)
        {
            EventClusterDAO eventCluster = await _repoEventClusters.GetItemAsync(eventClusterId);
            return _mapper.Map<EventClusterModel>(eventCluster);
        }

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

            //Only keep the first eventCluster for each device + event type sorted by start date descending
            /*var groupedEventClusters = 
                eventClusters
                .OrderByDescending(p => p.StartDate)
                .GroupBy(x => new { x.Device.DeviceId, x.EventType })
                .Select(p => p.First());*/
            //Order by start data descending
            var orderedEventClusters = eventClusters.OrderByDescending(p => p.StartDate);


            return _mapper.Map<IEnumerable<EventClusterModel>>(orderedEventClusters);
        }

        public async Task<IEnumerable<Guid>> GetClustersInRadius(EventClusterGeolocationModel eventClusterGeolocationObj)
        {
            if (eventClusterGeolocationObj == null || eventClusterGeolocationObj.ResponseEpicenterLocation == null)
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
            GeolocationDAOObject daoGeocodeCenterPoint = _mapper.Map<GeolocationDAOObject>(eventClusterGeolocationObj.ResponseEpicenterLocation);
            foreach (EventClusterDAO eventClusterObj in eventClusters)
                if (RadiusHelper.IsWithinRadius(eventClusterObj.Device.Geolocation, daoGeocodeCenterPoint, eventClusterGeolocationObj.Radius))
                    output.Add(new Guid(eventClusterObj.Id));
            return output;
        }

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
