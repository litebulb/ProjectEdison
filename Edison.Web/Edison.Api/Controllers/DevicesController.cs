using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Edison.Api.Config;
using Edison.Api.Helpers;
using Edison.Common.Interfaces;
using Edison.Common.Messages;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace Edison.Api.Controllers
{
    [ApiController]
    [Route("api/Devices")]
    public class DevicesController : ControllerBase
    {
        private readonly WebApiOptions _config;
        private readonly DevicesDataManager _devicesDataManager;
        private readonly EventClustersDataManager _eventClustersDataManager;
        private readonly IMassTransitServiceBus _serviceBus;

        public DevicesController(DevicesDataManager eventDataManager, EventClustersDataManager eventClustersDataManager,
            IOptions<WebApiOptions> config, IMassTransitServiceBus serviceBus)
        {
            _config = config.Value;
            _devicesDataManager = eventDataManager;
            _eventClustersDataManager = eventClustersDataManager;
            _serviceBus = serviceBus;
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpGet("{deviceId}")]
        [Produces(typeof(DeviceModel))]
        public async Task<IActionResult> GetDevice(Guid deviceId)
        {
            DeviceModel device = await _devicesDataManager.GetDevice(deviceId);
            return Ok(device);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpGet("Mobile/{userId}")]
        [Produces(typeof(DeviceModel))]
        public async Task<IActionResult> GetMobileDeviceFromUserId(string userId)
        {
            DeviceModel device = await _devicesDataManager.GetMobileDeviceFromUserId(userId);
            return Ok(device);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpGet("Map")]
        [Produces(typeof(IEnumerable<DeviceMapModel>))]
        public async Task<IActionResult> GetDevicesForMap()
        {
            IEnumerable<DeviceMapModel> devices = await _devicesDataManager.GetDevicesForMap();
            return Ok(devices);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "SuperAdmin")]
        [HttpPost("Radius")]
        [Produces(typeof(IEnumerable<Guid>))]
        public async Task<IActionResult> GetDevicesInRadius([FromBody] DeviceGeolocationModel deviceGeolocationObj)
        {
            IEnumerable<Guid> deviceIds = await _devicesDataManager.GetDevicesInRadius(deviceGeolocationObj);
            return Ok(deviceIds);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpPost("IsInBoundaries")]
        [Produces(typeof(bool))]
        public async Task<IActionResult> IsInBoundaries([FromBody] DeviceBoundaryGeolocationModel deviceBoundaryGeolocationObj)
        {
            var result = await _devicesDataManager.IsInBoundaries(deviceBoundaryGeolocationObj, _config.Boundaries.Epicenter, _config.Boundaries.Radius);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpGet]
        [Produces(typeof(IEnumerable<DeviceModel>))]
        public async Task<IActionResult> GetDevices()
        {
            IEnumerable<DeviceModel> devices = await _devicesDataManager.GetDevices();
            return Ok(devices);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "SuperAdmin")]
        [HttpPost]
        [Produces(typeof(DeviceModel))]
        public async Task<IActionResult> CreateOrUpdateDevice([FromBody]DeviceTwinModel deviceTwinObj)
        {
            var result = await _devicesDataManager.CreateOrUpdateDevice(deviceTwinObj);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "SuperAdmin")]
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

        [Authorize(AuthenticationSchemes = "AzureAd,B2CWeb", Policy = "Consumer")]
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

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "SuperAdmin")]
        [HttpDelete]
        public async Task<IActionResult> DeleteDevice(Guid deviceId)
        {
            var result = await _devicesDataManager.DeleteDevice(deviceId);
            return Ok(result);
        }
    }
}
