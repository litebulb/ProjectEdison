using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Edison.Api.Helpers;
using Edison.Core.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edison.Api.Controllers
{
    [Authorize(AuthenticationSchemes = "Backend,B2CWeb")]
    [Route("api/Devices")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly DevicesDataManager _devicesDataManager;

        public DevicesController(DevicesDataManager eventDataManager)
        {
            _devicesDataManager = eventDataManager;
        }

        [HttpGet("{deviceId}")]
        [Produces(typeof(DeviceModel))]
        public async Task<IActionResult> GetDevice(Guid deviceId)
        {
            DeviceModel events = await _devicesDataManager.GetDevice(deviceId);
            return Ok(events);
        }

        [HttpGet("Map")]
        [Produces(typeof(IEnumerable<DeviceMapModel>))]
        public async Task<IActionResult> GetDevicesForMap()
        {
            IEnumerable<DeviceMapModel> devices = await _devicesDataManager.GetDevicesForMap();
            return Ok(devices);
        }

        [HttpPost("Radius")]
        [Produces(typeof(IEnumerable<Guid>))]
        public async Task<IActionResult> GetDevicesInRadius([FromBody] DeviceGeolocationModel deviceGeolocationObj)
        {
            IEnumerable<Guid> deviceIds = await _devicesDataManager.GetDevicesInRadius(deviceGeolocationObj);
            return Ok(deviceIds);
        }

        [HttpGet]
        [Produces(typeof(IEnumerable<DeviceModel>))]
        public async Task<IActionResult> GetDevices()
        {
            IEnumerable<DeviceModel> devices = await _devicesDataManager.GetDevices();
            return Ok(devices);
        }

        [HttpPost]
        [Produces(typeof(DeviceModel))]
        public async Task<IActionResult> CreateOrUpdateDevice([FromBody]DeviceTwinModel deviceTwinObj)
        {
            var result = await _devicesDataManager.CreateOrUpdateDevice(deviceTwinObj);
            return Ok(result);
        }

        [HttpPut("Heartbeat")]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> UpdateHeartbeat([FromBody]Guid deviceId)
        {
            var result = await _devicesDataManager.UpdateHeartbeat(deviceId);
            return Ok(result);
        }

        [HttpDelete]
        [Produces(typeof(HttpStatusCode))]
        public async Task<IActionResult> DeleteDevice(Guid deviceId)
        {
            var result = await _devicesDataManager.DeleteDevice(deviceId);
            return Ok(result);
        }
    }
}
