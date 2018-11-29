using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Edison.Api.Helpers;

namespace Edison.Api.Controllers
{
    /// <summary>
    /// Controller to handle operations on Event Clusters
    /// </summary>
    [ApiController]
    [Route("api/EventClusters")]
    public class EventClustersController : ControllerBase
    {
        private readonly EventClustersDataManager _eventDataManager;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public EventClustersController(EventClustersDataManager eventDataManager)
        {
            _eventDataManager = eventDataManager;
        }

        /// <summary>
        /// Get Event Cluster by Id
        /// </summary>
        /// <param name="eventClusterId">Event Cluster Id</param>
        /// <returns>EventClusterModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpGet("{eventClusterId}")]
        [Produces(typeof(EventClusterModel))]
        public async Task<IActionResult> GetEventCluster(Guid eventClusterId)
        {
            EventClusterModel eventObj = await _eventDataManager.GetEventCluster(eventClusterId);
            return Ok(eventObj);
        }

        /// <summary>
        /// Get Event Clusters
        /// Only the last 3 events are returned
        /// </summary>
        /// <returns>List of Event Clusters</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpGet]
        [Produces(typeof(IEnumerable<EventClusterModel>))]
        public async Task<IActionResult> GetEventClusters()
        {
            IEnumerable<EventClusterModel> events = await _eventDataManager.GetEventClusters();
            return Ok(events);
        }

        /// <summary>
        /// Get a list of Event Clusters in a specific geolocation radius
        /// </summary>
        /// <param name="eventClusterGeolocationObj">EventClusterGeolocationModel</param>
        /// <returns>List of Event Clusters Ids</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpPost("Radius")]
        [Produces(typeof(IEnumerable<Guid>))]
        public async Task<IActionResult> GetClustersInRadius([FromBody] EventClusterGeolocationModel eventClusterGeolocationObj)
        {
            IEnumerable<Guid> eventClusterIds = await _eventDataManager.GetClustersInRadius(eventClusterGeolocationObj);
            return Ok(eventClusterIds);
        }

        /// <summary>
        /// Create or Update an Event Cluster
        /// </summary>
        /// <param name="eventObj">EventClusterCreationModel</param>
        /// <returns>EventClusterModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpPost]
        [Produces(typeof(EventClusterModel))]
        public async Task<IActionResult> CreateOrUpdateEventCluster([FromBody]EventClusterCreationModel eventObj)
        {
            var result = await _eventDataManager.CreateOrUpdateEventCluster(eventObj);
            return Ok(result);
        }

        /// <summary>
        /// Close an Event Cluster
        /// </summary>
        /// <param name="eventObj">EventClusterCloseModel</param>
        /// <returns>EventClusterModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpPut("Close")]
        [Produces(typeof(EventClusterModel))]
        public async Task<IActionResult> CloseEventCluster([FromBody]EventClusterCloseModel eventObj)
        {
            var result = await _eventDataManager.CloseEventCluster(eventObj);
            return Ok(result);
        }
    }
}
