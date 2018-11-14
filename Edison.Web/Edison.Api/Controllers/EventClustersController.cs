using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edison.Api.Helpers;
using Edison.Core.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edison.Api.Controllers
{
    [ApiController]
    [Route("api/EventClusters")]
    public class EventClustersController : ControllerBase
    {
        private readonly EventClustersDataManager _eventDataManager;

        public EventClustersController(EventClustersDataManager eventDataManager)
        {
            _eventDataManager = eventDataManager;
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpGet("{eventClusterId}")]
        [Produces(typeof(EventClusterModel))]
        public async Task<IActionResult> GetEventCluster(Guid eventClusterId)
        {
            EventClusterModel eventObj = await _eventDataManager.GetEventCluster(eventClusterId);
            return Ok(eventObj);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpGet]
        [Produces(typeof(IEnumerable<EventClusterModel>))]
        public async Task<IActionResult> GetEventClusters()
        {
            IEnumerable<EventClusterModel> events = await _eventDataManager.GetEventClusters();
            return Ok(events);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "SuperAdmin")]
        [HttpPost("Radius")]
        [Produces(typeof(IEnumerable<Guid>))]
        public async Task<IActionResult> GetClustersInRadius([FromBody] EventClusterGeolocationModel eventClusterGeolocationObj)
        {
            IEnumerable<Guid> eventClusterIds = await _eventDataManager.GetClustersInRadius(eventClusterGeolocationObj);
            return Ok(eventClusterIds);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "SuperAdmin")]
        [HttpPost]
        [Produces(typeof(EventClusterModel))]
        public async Task<IActionResult> CreateOrUpdateEventCluster([FromBody]EventClusterCreationModel eventObj)
        {
            var result = await _eventDataManager.CreateOrUpdateEventCluster(eventObj);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpPut("Close")]
        [Produces(typeof(EventClusterModel))]
        public async Task<IActionResult> CloseEventCluster([FromBody]EventClusterCloseModel eventObj)
        {
            var result = await _eventDataManager.CloseEventCluster(eventObj);
            return Ok(result);
        }
    }
}
