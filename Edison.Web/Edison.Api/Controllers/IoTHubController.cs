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
    [Authorize(AuthenticationSchemes = "Backend,B2CWeb")]
    [Route("api/IoTHub")]
    [ApiController]
    public class IoTHubController : ControllerBase
    {
        private readonly IoTHubControllerDataManager _iotHubControllerDataManager;

        public IoTHubController(IoTHubControllerDataManager iotHubControllerDataManager)
        {
            _iotHubControllerDataManager = iotHubControllerDataManager;
        }

        [HttpPost]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> CreationDevice(DeviceCreationModel device)
        {
            var result = await _iotHubControllerDataManager.CreationDevice(device);
            return Ok(result);
        }

        [HttpPut]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> UpdateDevice(DeviceUpdateModel device)
        {
            var result = await _iotHubControllerDataManager.UpdateDevice(device);
            return Ok(result);
        }

        [HttpPut("Tags")]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> UpdateDevicesTags(DevicesUpdateTagsModel devices)
        {
            var result = await _iotHubControllerDataManager.UpdateDevicesTags(devices);
            return Ok(result);
        }

        [HttpPut("Desired")]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> UpdateDevicesDesired(DevicesUpdateDesiredModel devices)
        {
            var result = await _iotHubControllerDataManager.UpdateDevicesDesired(devices);
            return Ok(result);
        }

        [HttpPut("DirectMethods")]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> LaunchDevicesDirectMethod(DevicesLaunchDirectMethodModel devices)
        {
            var result = await _iotHubControllerDataManager.LaunchDevicesDirectMethods(devices);
            return Ok(result);
        }

        [HttpDelete]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> DeleteDevice(Guid deviceId)
        {
            var result = await _iotHubControllerDataManager.DeleteDevice(deviceId);
            return Ok(result);
        }
    }
}
