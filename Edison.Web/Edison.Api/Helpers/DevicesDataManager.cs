using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using AutoMapper;
using Edison.Core.Common.Models;
using Edison.Common.Interfaces;
using Edison.Common.DAO;

namespace Edison.Api.Helpers
{
    /// <summary>
    /// Manager for the Device repository
    /// </summary>
    public class DevicesDataManager
    {
        private readonly ICosmosDBRepository<DeviceDAO> _repoDevices;
        private readonly IMapper _mapper;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public DevicesDataManager(IMapper mapper, ICosmosDBRepository<DeviceDAO> repoDevices)
        {
            _mapper = mapper;
            _repoDevices = repoDevices;
        }

        /// <summary>
        /// Get a device from a device id
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <returns>DeviceModel</returns>
        public async Task<DeviceModel> GetDevice(Guid deviceId)
        {
            DeviceDAO deviceEntity = await _repoDevices.GetItemAsync(deviceId);
            return _mapper.Map<DeviceModel>(deviceEntity);
        }

        /// <summary>
        /// Get a mobile device from a user id
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>DeviceModel</returns>
        public async Task<DeviceModel> GetMobileDeviceFromUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new Exception($"UserId not found");

            DeviceDAO deviceEntity = await _repoDevices.GetItemAsync(p => p.DeviceType == "Mobile" && p.Custom != null &&
            p.Custom["Email"] != null && (string)p.Custom["Email"] == userId);
            return _mapper.Map<DeviceModel>(deviceEntity);
        }

        /// <summary>
        /// Get the list of devices in a light model for map display
        /// </summary>
        /// <returns>List of devices</returns>
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

        /// <summary>
        /// Get the list of devices
        /// </summary>
        /// <param name="includeDisabled">Include devices where Enabled = false</param>
        /// <returns>List of devices</returns>
        public async Task<IEnumerable<DeviceModel>> GetDevices(bool includeDisabled)
        {
            IEnumerable<DeviceDAO> devices = await _repoDevices.GetItemsAsync(p => (includeDisabled || p.Enabled) && p.IoTDevice);
            return _mapper.Map<IEnumerable<DeviceModel>>(devices);
        }

        /// <summary>
        /// Get the list of devices in a specific radius
        /// </summary>
        /// <param name="pointLocation">Geolocation</param>
        /// <param name="radius">Radius in km</param>
        /// <param name="includeDisabled">Include devices where Enabled = false</param>
        /// <returns></returns>
        public async Task<IEnumerable<DeviceModel>> GetDevicesNearby(Geolocation pointLocation, double radius, bool includeDisabled)
        {
            IEnumerable<DeviceDAO> devices = await _repoDevices.GetItemsAsync(p => (includeDisabled || p.Enabled) && p.IoTDevice,
               p => new DeviceDAO()
               {
                   Id = p.Id,
                   Geolocation = p.Geolocation
               }
               );

            List<DeviceDAO> output = new List<DeviceDAO>();
            GeolocationDAOObject daoGeocodeCenterPoint = _mapper.Map<GeolocationDAOObject>(pointLocation);
            if (devices != null)
            {
                foreach (DeviceDAO deviceObj in devices)
                    if (deviceObj.Geolocation != null)
                        if (RadiusHelper.IsWithinRadius(deviceObj.Geolocation, daoGeocodeCenterPoint, radius))
                            output.Add(deviceObj);
            }
            return _mapper.Map<IEnumerable<DeviceModel>>(output);
        }

        /// <summary>
        /// Get the list of devices in a specific radius
        /// </summary>
        /// <param name="deviceGeolocationObj">DeviceGeolocationModel</param>
        /// <returns>List of device ids</returns>
        public async Task<IEnumerable<Guid>> GetDevicesInRadius(DeviceGeolocationModel deviceGeolocationObj)
        {
            IEnumerable<DeviceDAO> devices = await _repoDevices.GetItemsAsync(
               p => p.Enabled && ((!string.IsNullOrEmpty(deviceGeolocationObj.DeviceType) && p.DeviceType.ToLower() == deviceGeolocationObj.DeviceType.ToLower()) ||
               (string.IsNullOrEmpty(deviceGeolocationObj.DeviceType))),
               p => new DeviceDAO()
               {
                   Id = p.Id,
                   Geolocation = p.Geolocation
               }
               );

            List<Guid> output = new List<Guid>();
            GeolocationDAOObject daoGeocodeCenterPoint = _mapper.Map<GeolocationDAOObject>(deviceGeolocationObj.ResponseGeolocationPointLocation);
            if (devices != null)
            {
                foreach (DeviceDAO deviceObj in devices)
                    if(deviceObj.Geolocation != null)
                        if (RadiusHelper.IsWithinRadius(deviceObj.Geolocation, daoGeocodeCenterPoint, deviceGeolocationObj.Radius))
                            output.Add(new Guid(deviceObj.Id));
            }
            return output;
        }

        /// <summary>
        /// Determine if a device location is within the radius of a epicente
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="geolocationPoint">Geolocation of the geolocationPoint</param>
        /// <param name="radius">Radius in kilometer</param>
        /// <returns>True if the device is within the radius</returns>
        public async Task<bool> IsInBoundaries(string userId, Geolocation geolocationPoint, double radius)
        {
            if (string.IsNullOrEmpty(userId))
                throw new Exception($"The userId was not found.");

            DeviceModel device = await GetMobileDeviceFromUserId(userId);
            return RadiusHelper.IsWithinRadius(device.Geolocation, geolocationPoint, radius);
        }

        /// <summary>
        /// Create or update a device
        /// </summary>
        /// <param name="deviceTwinObj">DeviceTwinModel</param>
        /// <returns>DeviceModel</returns>
        public async Task<DeviceModel> CreateOrUpdateDevice(DeviceTwinModel deviceTwinObj)
        {
            if (deviceTwinObj.DeviceId == Guid.Empty)
                throw new Exception($"No device found that matches DeviceId: {deviceTwinObj.DeviceId}");

            DeviceDAO deviceDAO = await _repoDevices.GetItemAsync(deviceTwinObj.DeviceId);

            //Create
            if (deviceDAO == null)
                return await CreateDevice(deviceTwinObj);

            //Update
            deviceDAO.IoTDevice = true;
            if (deviceTwinObj.Properties?.Desired != null)
                deviceDAO.Desired = deviceTwinObj.Properties.Desired;
            if (deviceTwinObj.Properties?.Reported != null)
                deviceDAO.Reported = deviceTwinObj.Properties.Reported;
            if (deviceTwinObj.Tags != null)
            {
                deviceDAO.DeviceType = deviceTwinObj.Tags.DeviceType;
                deviceDAO.Enabled = deviceTwinObj.Tags.Enabled;
                deviceDAO.Custom = deviceTwinObj.Tags.Custom;
                deviceDAO.Name = deviceTwinObj.Tags.Name;
                deviceDAO.Location1 = deviceTwinObj.Tags.Location1;
                deviceDAO.Location2 = deviceTwinObj.Tags.Location2;
                deviceDAO.Location3 = deviceTwinObj.Tags.Location3;
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

        /// <summary>
        /// Create or update a mobile device
        /// </summary>
        /// <param name="deviceMobile">DeviceMobileModel</param>
        /// <returns>DeviceMobileModel</returns>
        public async Task<DeviceMobileModel> CreateOrUpdateDevice(DeviceMobileModel deviceMobile)
        {
            if (deviceMobile.DeviceId == Guid.Empty  && string.IsNullOrEmpty(deviceMobile.MobileId))
                throw new Exception($"Invalid DeviceId and/or MobileId");

            DeviceDAO deviceDAO = null;

            if (deviceMobile.DeviceId != Guid.Empty)
                deviceDAO = await _repoDevices.GetItemAsync(deviceMobile.DeviceId);
            else if (!string.IsNullOrEmpty(deviceMobile.MobileId))
                deviceDAO = await _repoDevices.GetItemAsync(
                    d => (string)d.Custom["MobileId"] == deviceMobile.MobileId);


            if (deviceDAO == null)
            //Create
            {
                deviceDAO = await CreateMobileDevice(deviceMobile);
            }   
            else
            //Update
            {
                deviceDAO.Custom = deviceMobile.Custom;
                try
                {
                    await _repoDevices.UpdateItemAsync(deviceDAO);
                }
                catch (DocumentClientException e)
                {
                    //Update concurrency issue, retrying
                    if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                        return await CreateOrUpdateDevice(deviceMobile);
                    throw e;
                }
            }

            return _mapper.Map<DeviceMobileModel>(deviceDAO);
        }

        /// <summary>
        /// Create a device
        /// </summary>
        /// <param name="deviceTwinObj">DeviceTwinModel</param>
        /// <returns>DeviceModel</returns>
        public async Task<DeviceModel> CreateDevice(DeviceTwinModel deviceTwinObj)
        {
            //If device doesn't exist, throw exception
            DeviceDAO deviceEntity = _mapper.Map<DeviceDAO>(deviceTwinObj);
            deviceEntity.Id = await _repoDevices.CreateItemAsync(deviceEntity);
            if (_repoDevices.IsDocumentKeyNull(deviceEntity))
                throw new Exception($"An error occured when creating a new device: {deviceTwinObj.DeviceId}");

            return _mapper.Map<DeviceModel>(deviceEntity);
        }

        /// <summary>
        /// Create a mobile device
        /// </summary>
        /// <param name="deviceMobileObj">DeviceMobileModel</param>
        /// <returns>DeviceDAO</returns>
        public async Task<DeviceDAO> CreateMobileDevice(DeviceMobileModel deviceMobileObj)
        {
            //If device doesn't exist, throw exception
            DeviceDAO deviceEntity = _mapper.Map<DeviceDAO>(deviceMobileObj);
            deviceEntity.Id = null;
            deviceEntity.Id = await _repoDevices.CreateItemAsync(deviceEntity);
            if (_repoDevices.IsDocumentKeyNull(deviceEntity))
                throw new Exception($"An error occured when creating a new device: {deviceMobileObj.DeviceId}");

            return deviceEntity;
        }

        /// <summary>
        /// Update the LastAccessTime of a device
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <returns>DeviceHeartbeatUpdatedModel</returns>
        public async Task<DeviceHeartbeatUpdatedModel> UpdateHeartbeat(Guid deviceId)
        {
            if (deviceId == Guid.Empty)
                throw new Exception($"No device found that matches DeviceId: {deviceId}");

            DeviceDAO deviceDAO = await _repoDevices.GetItemAsync(deviceId);

            string etag = deviceDAO.ETag;
            deviceDAO.LastAccessTime = DateTime.UtcNow;

            try
            {
                await _repoDevices.UpdateItemAsync(deviceDAO);
                return new DeviceHeartbeatUpdatedModel()
                {
                    Device = _mapper.Map<DeviceModel>(deviceDAO),
                    NeedsUpdate = deviceDAO.Enabled
                };
            }
            catch (DocumentClientException e)
            {
                //Update concurrency issue, retrying
                if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                    return await UpdateHeartbeat(deviceId);
                throw e;
            }
        }

        /// <summary>
        /// Update the geolocation of a mobile device
        /// </summary>
        /// <param name="geolocation">Geolocation of the device</param>
        /// <param name="userId">UserId</param>
        /// <returns>DeviceGeolocationUpdateResultModel</returns>
        public async Task<DeviceGeolocationUpdateResultModel> UpdateMobileGeolocation(Geolocation geolocation, string userId)
        {
            if (geolocation == null)
                throw new Exception($"No Geolocation found: {geolocation}");
            if (string.IsNullOrEmpty(userId))
                throw new Exception($"UserId not found");

            DeviceDAO deviceDAO = await _repoDevices.GetItemAsync(p => p.DeviceType == "Mobile" && p.Custom != null && 
            p.Custom["Email"] != null && (string)p.Custom["Email"] == userId);

            if (deviceDAO != null)
            {
                if (deviceDAO.Geolocation != null)
                {
                    if (geolocation.Latitude == deviceDAO.Geolocation.Latitude &&
                        geolocation.Longitude == deviceDAO.Geolocation.Longitude)
                        return new DeviceGeolocationUpdateResultModel() { Success = true };
                }

                string etag = deviceDAO.ETag;
                deviceDAO.LastAccessTime = DateTime.UtcNow;
                deviceDAO.Geolocation = _mapper.Map<GeolocationDAOObject>(geolocation);

                try
                {
                    var result = await _repoDevices.UpdateItemAsync(deviceDAO);
                    if (result)
                    {
                        return new DeviceGeolocationUpdateResultModel() {
                            Success = true,
                            Device = deviceDAO.Enabled ? _mapper.Map<DeviceModel>(deviceDAO) : null
                        };
                    }
                    throw new Exception($"Error while updating device geolocation: {userId}.");

                }
                catch (DocumentClientException e)
                {
                    //Update concurrency issue, retrying
                    if (e.StatusCode == HttpStatusCode.PreconditionFailed)
                        return await UpdateMobileGeolocation(geolocation, userId);
                    throw e;
                }
            }
            return new DeviceGeolocationUpdateResultModel() { Success = false };
        }

        /// <summary>
        /// Delete a device
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <returns>True if the device was successfully deleted</returns>
        public async Task<bool> DeleteDevice(Guid deviceId)
        {
            if (await _repoDevices.GetItemAsync(deviceId) != null)
                return await _repoDevices.DeleteItemAsync(deviceId);
            return true;
        }

        /// <summary>
        /// Delete a mobile device
        /// </summary>
        /// <param name="registrationId">RegistrationId</param>
        /// <returns>True if the device was successfully deleted</returns>
        public async Task<bool> DeleteMobileDevice(string registrationId)
        {
            DeviceDAO device = await _repoDevices.GetItemAsync(d => (string)d.Custom["RegistrationId"] == registrationId);
            if (device != null)
                return await _repoDevices.DeleteItemAsync(device.Id);
            return true;
        }
    }
}
