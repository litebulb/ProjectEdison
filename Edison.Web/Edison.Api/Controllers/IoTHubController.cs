using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Edison.Api.Helpers;

namespace Edison.Api.Controllers
{
    /// <summary>
    /// Controller to handle operations on Iot Hub devices
    /// </summary>
    [ApiController]
    [Route("api/IoTHub")]
    public class IoTHubController : ControllerBase
    {
        private readonly IoTHubControllerDataManager _iotHubControllerDataManager;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public IoTHubController(IoTHubControllerDataManager iotHubControllerDataManager)
        {
            _iotHubControllerDataManager = iotHubControllerDataManager;
        }

        /// <summary>
        /// Create a device
        /// Call for debug only. Device Creation should be done through DPS only
        /// </summary>
        /// <param name="device">DeviceCreationModel</param>
        /// <returns>True if the masstransit publish command has succeeded</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpPost]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> CreationDevice(DeviceCreationModel device)
        {
            var result = await _iotHubControllerDataManager.CreationDevice(device);
            return Ok(result);
        }

        /// <summary>
        /// Update a device
        /// </summary>
        /// <param name="device">DeviceUpdateModel</param>
        /// <returns>True if the masstransit publish command has succeeded</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpPut]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> UpdateDevice(DeviceUpdateModel device)
        {
            var result = await _iotHubControllerDataManager.UpdateDevice(device);
            return Ok(result);
        }

        /// <summary>
        /// Update a set of devices tags
        /// </summary>
        /// <param name="devices">List of device ids</param>
        /// <returns>True if the masstransit publish command has succeeded</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpPut("Tags")]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> UpdateDevicesTags(DevicesUpdateTagsModel devices)
        {
            var result = await _iotHubControllerDataManager.UpdateDevicesTags(devices);
            return Ok(result);
        }

        /// <summary>
        /// Update a set of devices desired properties
        /// </summary>
        /// <param name="devices">List of device ids</param>
        /// <returns>True if the masstransit publish command has succeeded</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpPut("Desired")]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> UpdateDevicesDesired(DevicesUpdateDesiredModel devices)
        {
            var result = await _iotHubControllerDataManager.UpdateDevicesDesired(devices);
            return Ok(result);
        }

        /// <summary>
        /// Launch a direct method on a set of devices
        /// </summary>
        //// <param name="devices">List of device ids</param>
        /// <returns>True if the masstransit publish command has succeeded</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpPut("DirectMethods")]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> LaunchDevicesDirectMethod(DevicesLaunchDirectMethodModel devices)
        {
            var result = await _iotHubControllerDataManager.LaunchDevicesDirectMethods(devices);
            return Ok(result);
        }

        /// <summary>
        /// Delete a device
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <returns>True if the masstransit publish command has succeeded</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpDelete]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> DeleteDevice(Guid deviceId)
        {
            var result = await _iotHubControllerDataManager.DeleteDevice(deviceId);
            return Ok(result);
        }
    }
}
