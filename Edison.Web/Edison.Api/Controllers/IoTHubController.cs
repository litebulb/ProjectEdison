using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Edison.Api.Helpers;
using Edison.Common.Interfaces;
using Edison.Common.Messages;
using Edison.Core.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edison.Api.Controllers
{
    [ApiController]
    [Route("api/IoTHub")]
    public class IoTHubController : ControllerBase
    {
        private readonly IoTHubControllerDataManager _iotHubControllerDataManager;

        public IoTHubController(IoTHubControllerDataManager iotHubControllerDataManager)
        {
            _iotHubControllerDataManager = iotHubControllerDataManager;
        }

        //Call for debug only. Device Creation should be done through DPS only
        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "SuperAdmin")]
        [HttpPost]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> CreationDevice(DeviceCreationModel device)
        {
            var result = await _iotHubControllerDataManager.CreationDevice(device);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpPut]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> UpdateDevice(DeviceUpdateModel device)
        {
            var result = await _iotHubControllerDataManager.UpdateDevice(device);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpPut("Tags")]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> UpdateDevicesTags(DevicesUpdateTagsModel devices)
        {
            var result = await _iotHubControllerDataManager.UpdateDevicesTags(devices);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpPut("Desired")]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> UpdateDevicesDesired(DevicesUpdateDesiredModel devices)
        {
            var result = await _iotHubControllerDataManager.UpdateDevicesDesired(devices);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpPut("DirectMethods")]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> LaunchDevicesDirectMethod(DevicesLaunchDirectMethodModel devices)
        {
            var result = await _iotHubControllerDataManager.LaunchDevicesDirectMethods(devices);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpDelete]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> DeleteDevice(Guid deviceId)
        {
            var result = await _iotHubControllerDataManager.DeleteDevice(deviceId);
            return Ok(result);
        }
    }
}
