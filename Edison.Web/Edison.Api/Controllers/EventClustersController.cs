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
    [Authorize(AuthenticationSchemes = "Backend,B2CWeb")]
    [Route("api/EventClusters")]
    [ApiController]
    public class EventClustersController : ControllerBase
    {
        private readonly EventClustersDataManager _eventDataManager;

        public EventClustersController(EventClustersDataManager eventDataManager)
        {
            _eventDataManager = eventDataManager;
        }

        [HttpGet("{eventClusterId}")]
        [Produces(typeof(EventClusterModel))]
        public async Task<IActionResult> GetEventCluster(Guid eventClusterId)
        {
            EventClusterModel eventObj = await _eventDataManager.GetEventCluster(eventClusterId);
            return Ok(eventObj);
        }

        [HttpGet]
        [Produces(typeof(IEnumerable<EventClusterModel>))]
        public async Task<IActionResult> GetEventClusters()
        {
            IEnumerable<EventClusterModel> events = await _eventDataManager.GetEventClusters();
            return Ok(events);
        }

        [HttpPost("Radius")]
        [Produces(typeof(IEnumerable<Guid>))]
        public async Task<IActionResult> GetClustersInRadius([FromBody] EventClusterGeolocationModel eventClusterGeolocationObj)
        {
            IEnumerable<Guid> eventClusterIds = await _eventDataManager.GetClustersInRadius(eventClusterGeolocationObj);
            return Ok(eventClusterIds);
        }

        [HttpPost]
        [Produces(typeof(EventClusterModel))]
        public async Task<IActionResult> CreateOrUpdateEventCluster([FromBody]EventClusterCreationModel eventObj)
        {
            var result = await _eventDataManager.CreateOrUpdateEventCluster(eventObj);
            return Ok(result);
        }

        [HttpPut("Close")]
        [Produces(typeof(EventClusterModel))]
        public async Task<IActionResult> CloseEventCluster([FromBody]EventClusterCloseModel eventObj)
        {
            var result = await _eventDataManager.CloseEventCluster(eventObj);
            return Ok(result);
        }
    }
}
