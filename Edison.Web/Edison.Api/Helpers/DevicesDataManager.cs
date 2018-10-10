using Edison.Core.Common.Models;
using System.Threading.Tasks;
using AutoMapper;
using System;
using Edison.Common.DAO;
using Edison.Common.Interfaces;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Edison.Api.Config;
using Microsoft.Azure.Documents;
using System.Net;

namespace Edison.Api.Helpers
{
    public class DevicesDataManager
    {
        private ICosmosDBRepository<DeviceDAO> _repoDevices;
        private IMapper _mapper;

        public DevicesDataManager(IMapper mapper,
            ICosmosDBRepository<DeviceDAO> repoDevices)
        {
            _mapper = mapper;
            _repoDevices = repoDevices;
        }

        public async Task<DeviceModel> GetDevice(Guid deviceId)
        {
            DeviceDAO deviceEntity = await _repoDevices.GetItemAsync(deviceId);
            return _mapper.Map<DeviceModel>(deviceEntity);
        }

        public async Task<IEnumerable<DeviceMapModel>> GetDevicesForMap()
        {
            IEnumerable<DeviceDAO> devices = await _repoDevices.GetItemsAsync(
                p => p.Enabled && p.Sensor,
                p => new DeviceDAO()
                {
                    Id = p.Id,
                    DeviceType = p.DeviceType,
                    Geolocation = p.Geolocation,
                    LastAccessTime = p.LastAccessTime
                }
                );

            return _mapper.Map<IEnumerable<DeviceMapModel>>(devices);
        }

        public async Task<IEnumerable<DeviceModel>> GetDevices()
        {
            IEnumerable<DeviceDAO> devices = await _repoDevices.GetItemsAsync(p => p.Enabled && p.Sensor);
            return _mapper.Map<IEnumerable<DeviceModel>>(devices);
        }

        public async Task<IEnumerable<Guid>> GetDevicesInRadius(DeviceGeolocationModel deviceGeolocationObj)
        {
            IEnumerable<DeviceDAO> devices = await _repoDevices.GetItemsAsync(
               p => p.Enabled && ((deviceGeolocationObj.FetchSensors && p.Sensor) || (!deviceGeolocationObj.FetchSensors && !p.Sensor)),
               p => new DeviceDAO()
               {
                   Id = p.Id,
                   Geolocation = p.Geolocation
               }
               );

            List<Guid> output = new List<Guid>();
            GeolocationDAOObject daoGeocodeCenterPoint = _mapper.Map<GeolocationDAOObject>(deviceGeolocationObj.ResponseEpicenterLocation);
            foreach (DeviceDAO deviceObj in devices)
                if (RadiusHelper.IsWithinRadius(deviceObj.Geolocation, daoGeocodeCenterPoint, deviceGeolocationObj.Radius))
                    output.Add(deviceObj.Id);
            return output;
        }

        public async Task<DeviceModel> CreateOrUpdateDevice(DeviceTwinModel deviceTwinObj)
        {
            if (deviceTwinObj.DeviceId == Guid.Empty)
                throw new Exception($"No device found that matches DeviceId: {deviceTwinObj.DeviceId}");

            DeviceDAO deviceDAO = await _repoDevices.GetItemAsync(deviceTwinObj.DeviceId);

            //Create
            if (deviceDAO == null)
                return await CreateDevice(deviceTwinObj);

            //Update
            if(deviceTwinObj.Properties?.Desired != null)
                deviceDAO.Desired = deviceTwinObj.Properties.Desired;
            if (deviceTwinObj.Properties?.Reported != null)
                deviceDAO.Reported = deviceTwinObj.Properties.Reported;
            if (deviceTwinObj.Tags != null)
            {
                deviceDAO.DeviceType = deviceTwinObj.Tags.DeviceType;
                deviceDAO.Enabled = deviceTwinObj.Tags.Enabled;
                deviceDAO.Custom = deviceTwinObj.Tags.Custom;
                deviceDAO.LocationName = deviceTwinObj.Tags.LocationName;
                deviceDAO.LocationLevel1 = deviceTwinObj.Tags.LocationLevel1;
                deviceDAO.LocationLevel2 = deviceTwinObj.Tags.LocationLevel2;
                deviceDAO.LocationLevel3 = deviceTwinObj.Tags.LocationLevel3;
                deviceDAO.Sensor = deviceTwinObj.Tags.Sensor;
                deviceDAO.Geolocation = _mapper.Map<GeolocationDAOObject>(deviceTwinObj.Tags.Geolocation);
            }

            try
            {
                await _repoDevices.UpdateItemAsync(deviceDAO);
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    return await CreateOrUpdateDevice(deviceTwinObj);
                throw e;
            }

            return _mapper.Map<DeviceModel>(deviceDAO);
        }

        public async Task<DeviceModel> CreateDevice(DeviceTwinModel deviceTwinObj)
        {
            //If device doesn't exist, throw exception
            DeviceDAO deviceEntity = _mapper.Map<DeviceDAO>(deviceTwinObj);
            deviceEntity.Id = await _repoDevices.CreateItemAsync(deviceEntity);
            if (deviceEntity.Id == Guid.Empty)
                throw new Exception($"An error occured when creating a new device: {deviceTwinObj.DeviceId}");

            return _mapper.Map<DeviceModel>(deviceEntity);
        }

        public async Task<bool> UpdateHeartbeat(Guid deviceId)
        {
            if (deviceId == Guid.Empty)
                throw new Exception($"No device found that matches DeviceId: {deviceId}");

            DeviceDAO deviceDAO = await _repoDevices.GetItemAsync(deviceId);

            string etag = deviceDAO.ETag;
            deviceDAO.LastAccessTime = DateTime.UtcNow;

            try
            {
                return await _repoDevices.UpdateItemAsync(deviceDAO);
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    return await UpdateHeartbeat(deviceId);
                throw e;
            }
        }

        public async Task<bool> DeleteDevice(Guid deviceId)
        {
            if (await _repoDevices.GetItemAsync(deviceId) != null)
                return await _repoDevices.DeleteItemAsync(deviceId);
            return true;
        }
    }
}
