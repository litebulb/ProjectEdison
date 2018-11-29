using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Edison.Common.Interfaces;
using Edison.Common.Messages;
using Edison.Api.Config;
using Edison.Api.Helpers;

namespace Edison.Api.Controllers
{
    /// <summary>
    /// Controller to handle operations on Devices
    /// </summary>
    [ApiController]
    [Route("api/Devices")]
    public class DevicesController : ControllerBase
    {
        private readonly WebApiOptions _config;
        private readonly DevicesDataManager _devicesDataManager;
        private readonly EventClustersDataManager _eventClustersDataManager;
        private readonly IMassTransitServiceBus _serviceBus;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public DevicesController(DevicesDataManager eventDataManager, EventClustersDataManager eventClustersDataManager,
            IOptions<WebApiOptions> config, IMassTransitServiceBus serviceBus)
        {
            _config = config.Value;
            _devicesDataManager = eventDataManager;
            _eventClustersDataManager = eventClustersDataManager;
            _serviceBus = serviceBus;
        }

        /// <summary>
        /// Get a device from a device id
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <returns>DeviceModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpGet("{deviceId}")]
        [Produces(typeof(DeviceModel))]
        public async Task<IActionResult> GetDevice(Guid deviceId)
        {
            DeviceModel device = await _devicesDataManager.GetDevice(deviceId);
            return Ok(device);
        }

        /// <summary>
        /// Get a mobile device from a user id
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>DeviceModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpGet("Mobile/{userId}")]
        [Produces(typeof(DeviceModel))]
        public async Task<IActionResult> GetMobileDeviceFromUserId(string userId)
        {
            DeviceModel device = await _devicesDataManager.GetMobileDeviceFromUserId(userId);
            return Ok(device);
        }

        /// <summary>
        /// Get the list of devices in a light model for map display
        /// </summary>
        /// <returns>List of devices</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpGet("Map")]
        [Produces(typeof(IEnumerable<DeviceMapModel>))]
        public async Task<IActionResult> GetDevicesForMap()
        {
            IEnumerable<DeviceMapModel> devices = await _devicesDataManager.GetDevicesForMap();
            return Ok(devices);
        }

        /// <summary>
        /// Get the list of devices
        /// </summary>
        /// <returns>List of devices</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpGet]
        [Produces(typeof(IEnumerable<DeviceModel>))]
        public async Task<IActionResult> GetDevices()
        {
            IEnumerable<DeviceModel> devices = await _devicesDataManager.GetDevices();
            return Ok(devices);
        }

        /// <summary>
        /// Get the list of devices in a specific radius
        /// </summary>
        /// <param name="deviceGeolocationObj">DeviceGeolocationModel</param>
        /// <returns>List of device ids</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpPost("Radius")]
        [Produces(typeof(IEnumerable<Guid>))]
        public async Task<IActionResult> GetDevicesInRadius([FromBody] DeviceGeolocationModel deviceGeolocationObj)
        {
            IEnumerable<Guid> deviceIds = await _devicesDataManager.GetDevicesInRadius(deviceGeolocationObj);
            return Ok(deviceIds);
        }

        /// <summary>
        /// Determine if a device location is within the radius of a epicente
        /// </summary>
        /// <returns>True if the device is within the radius</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Consumer)]
        [HttpGet("IsInBoundaries")]
        [Produces(typeof(bool))]
        public async Task<IActionResult> IsInBoundaries()
        {
            string userId = UserHelper.GetBestClaimValue(User.Claims.ToList(), _config.ClaimsId, true).ToLower();

            var result = await _devicesDataManager.IsInBoundaries(userId, _config.Boundaries.GeolocationPoint, _config.Boundaries.Radius);
            return Ok(result);
        }

        /// <summary>
        /// Create or update a device
        /// </summary>
        /// <param name="deviceTwinObj">DeviceTwinModel</param>
        /// <returns>DeviceModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpPost]
        [Produces(typeof(DeviceModel))]
        public async Task<IActionResult> CreateOrUpdateDevice([FromBody]DeviceTwinModel deviceTwinObj)
        {
            var result = await _devicesDataManager.CreateOrUpdateDevice(deviceTwinObj);
            return Ok(result);
        }

        /// <summary>
        /// Update the LastAccessTime of a device
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <returns>DeviceHeartbeatUpdatedModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpPut("Heartbeat")]
        [Produces(typeof(DeviceHeartbeatUpdatedModel))]
        public async Task<IActionResult> UpdateHeartbeat([FromBody]Guid deviceId)
        {
            var result = await _devicesDataManager.UpdateHeartbeat(deviceId);
            if(result != null)
                return Ok(result);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating the heartbeat.");
        }

        /// <summary>
        /// Update the geolocation of a mobile device
        /// </summary>
        /// <param name="geolocation">Geolocation of the device</param>
        /// <returns>200 if successful</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Consumer)]
        [HttpPut("DeviceLocation")]
        public async Task<IActionResult> UpdateMobileGeolocation([FromBody]DeviceGeolocationUpdateModel updateGeolocationObj)
        {
            string userId = UserHelper.GetBestClaimValue(User.Claims.ToList(), _config.ClaimsId, true).ToLower();

            //Update device location
            DeviceGeolocationUpdateResultModel resultDevice = await _devicesDataManager.UpdateMobileGeolocation(updateGeolocationObj.Geolocation, userId);
            if(resultDevice == null || !resultDevice.Success) //Error
                return StatusCode(StatusCodes.Status500InternalServerError);
            if (resultDevice.Device == null) //No device update
                return Ok(); 
           
            //Send device update
            if (_serviceBus != null && _serviceBus.BusAccess != null)
            {
                //We need to notify the frontend about the change, but it doesn't fit in the device workflow as phones aren't actual iot devices.
                await _serviceBus.BusAccess.Publish(new DeviceUIUpdateRequestedEvent()
                {
                    CorrelationId = resultDevice.Device.DeviceId,
                    DeviceUI = new DeviceUIModel()
                    {
                        DeviceId = resultDevice.Device.DeviceId,
                        Device = resultDevice.Device,
                        UpdateType = "UpdateDevice"
                    }
                });
            }

            //Update active cluster location, if any
            EventClusterGeolocationUpdateResultModel resultEventCluster = await _eventClustersDataManager
                .UpdateGeolocation(updateGeolocationObj.Geolocation, resultDevice.Device.DeviceId);
            if (resultEventCluster == null || !resultEventCluster.Success) //Error
                return StatusCode(StatusCodes.Status500InternalServerError);

            //Send event cluster update
            if (_serviceBus != null && _serviceBus.BusAccess != null)
            {
                //We need to notify the frontend about the change
                await _serviceBus.BusAccess.Publish(new EventUIUpdateRequestedEvent()
                {
                    CorrelationId = resultEventCluster.EventCluster.Device.DeviceId,
                    EventClusterUI = new EventClusterUIModel()
                    {
                        EventCluster = resultEventCluster.EventCluster,
                        UpdateType = "UpdateEventCluster"
                    }
                });
                
            }

            return Ok();
        }

        /// <summary>
        /// Delete a device
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <returns>True if the device was successfully deleted</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpDelete]
        public async Task<IActionResult> DeleteDevice(Guid deviceId)
        {
            var result = await _devicesDataManager.DeleteDevice(deviceId);
            return Ok(result);
        }
    }
}
