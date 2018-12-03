using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Edison.Common.Interfaces;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;
using Edison.Api.Config;
using Edison.Api.Helpers;

namespace Edison.Api.Controllers
{
    /// <summary>
    /// Controller to handle operations on responses
    /// </summary>
    [ApiController]
    [Route("api/Responses")]
    public class ResponsesController : ControllerBase
    {
        private readonly WebApiOptions _config;
        private readonly ResponseDataManager _responseDataManager;
        private readonly IMassTransitServiceBus _serviceBus;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public ResponsesController(ResponseDataManager responseDataManager,
            IOptions<WebApiOptions> config, IMassTransitServiceBus serviceBusClient)
        {
            _config = config.Value;
            _responseDataManager = responseDataManager;
            _serviceBus = serviceBusClient;
        }

        /// <summary>
        /// Get a response full object by Id
        /// </summary>
        /// <param name="responseId">Response Id</param>
        /// <returns>ResponseModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Consumer)]
        [HttpGet("{responseId}")]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> GetResponseDetail(Guid responseId)
        {
            ResponseModel responseObj = await _responseDataManager.GetResponse(responseId);
            return Ok(responseObj);
        }

        /// <summary>
        /// Get a list of responses light objects
        /// </summary>
        /// <returns>List of Response Light Model</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Consumer)]
        [HttpGet]
        [Produces(typeof(IEnumerable<ResponseLightModel>))]
        public async Task<IActionResult> GetResponses()
        {
            IEnumerable<ResponseLightModel> responseObjs = await _responseDataManager.GetResponses();
            return Ok(responseObjs);
        }

        /// <summary>
        /// Get a list of responses that are within the radius of a point. The size of the radius is the primary radius of the action plan
        /// </summary>
        /// <param name="responseGeolocationObj">ResponseGeolocationModel</param>
        /// <returns>List of Response Model</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Consumer)]
        [HttpPost("Radius")]
        [Produces(typeof(IEnumerable<ResponseModel>))]
        public async Task<IActionResult> GetResponsesFromPointRadius([FromBody]ResponseGeolocationModel responseGeolocationObj)
        {
            IEnumerable<ResponseModel> responseObjs = await _responseDataManager.GetResponsesFromPointRadius(responseGeolocationObj);
            return Ok(responseObjs);
        }

        /// <summary>
        /// Create a new response and start a response saga
        /// </summary>
        /// <param name="responseObj">ResponseCreationModel</param>
        /// <returns>ResponseModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpPost]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> CreateResponse([FromBody]ResponseCreationModel responseObj)
        {
            var result = await _responseDataManager.CreateResponse(responseObj);
            if (result != null)
            {
                IEventSagaReceiveResponseCreated newMessage = new EventSagaReceiveResponseCreated()
                {
                    Response = result
                };
                await _serviceBus.BusAccess.Publish(newMessage);
            }
            return Ok(result);
        }

        /// <summary>
        /// Set the safe status of the current user
        /// </summary>
        /// <param name="responseSafeUpdateObj">ResponseSafeUpdateModel</param>
        /// <returns>True if the call succeeded</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Consumer)]
        [HttpPut("Safe")]
        [Produces(typeof(bool))]
        public async Task<IActionResult> SetSafeStatus([FromBody]ResponseSafeUpdateModel responseSafeUpdateObj)
        {
            string userId = UserHelper.GetBestClaimValue(User.Claims.ToList(), _config.ClaimsId, true).ToLower();

            bool result = await _responseDataManager.SetSafeStatus(userId, responseSafeUpdateObj.IsSafe);
            return Ok(result);
        }

        /// <summary>
        /// Locate a response by adding a geolocation. The call will fail if a geolocation already exist.
        /// Trigger a new Masstransit message if the call succeeded
        /// </summary>
        /// <param name="responseObj">ResponseStartModel</param>
        /// <returns>Returns 200 if the call succeeded</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpPost("Locate")]
        public async Task<IActionResult> LocateResponse([FromBody]ResponseStartModel responseObj)
        {
            var result = await _responseDataManager.LocateResponse(new ResponseUpdateModel()
            {
                ResponseId = responseObj.ResponseId,
                Geolocation = responseObj.Geolocation
            });
            if (result != null)
            {
                IEventSagaReceiveResponseUpdated newMessage = new EventSagaReceiveResponseUpdated()
                {
                    Response = result
                };
                await _serviceBus.BusAccess.Publish(newMessage);
            }
            return Ok();
        }

        /// <summary>
        /// Close a response by adding a end date.
        /// Trigger a new Masstransit message if the call succeeded
        /// </summary>
        /// <param name="responseObj">EventSagaReceiveResponseClosed</param>
        /// <returns>ResponseModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpPut("Close")]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> CloseResponse(ResponseCloseModel responseObj)
        {
            var result = await _responseDataManager.CloseResponse(responseObj);
            IEventSagaReceiveResponseClosed newMessage = new EventSagaReceiveResponseClosed()
            {
                Response = result
            };
            await _serviceBus.BusAccess.Publish(newMessage);
            return Ok(result);
        }

        /// <summary>
        /// Associated a set of Event Clusters Ids to a response
        /// </summary>
        /// <param name="responseObj">ResponseEventClustersUpdateModel</param>
        /// <returns>ResponseModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpPut("AddEventClusters")]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> AddEventClusterIdsToResponse(ResponseEventClustersUpdateModel responseObj)
        {
            ResponseModel result = await _responseDataManager.AddEventClusterIdsToResponse(responseObj);
            return Ok(result);
        }

        /// <summary>
        /// Delete a response. This call is for debugging purposes only
        /// </summary>
        /// <param name="responseId">Response Id</param>
        /// <returns>true if the call succeeded</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpDelete]
        [Produces(typeof(bool))]
        public async Task<IActionResult> DeleteResponse(Guid responseId)
        {
            var result = await _responseDataManager.DeleteResponse(responseId);
            return Ok(result);
        }

        /// <summary>
        /// Complete an action and update the action object
        /// </summary>
        /// <param name="actionCompletionObj">ActionCompletionModel</param>
        /// <returns>true if the call succeeded</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpPost("CompleteAction")]
        [Produces(typeof(bool))]
        public async Task<IActionResult> CompleteAction(ActionCompletionModel actionCompletionObj)
        {
            var result = await _responseDataManager.CompleteAction(actionCompletionObj);
            return Ok(result);
        }

        /// <summary>
        /// Add action to a response
        /// Trigger a MassTransit message if the call succeeded
        /// </summary>
        /// <param name="responseObj">ResponseChangeActionPlanModel</param>
        /// <returns>ResponseModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [HttpPut("ChangeAction")]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> AddActionToResponse(ResponseChangeActionPlanModel responseObj)
        {
            ResponseModel result = await _responseDataManager.ChangeActionOnResponse(responseObj);
            if (result != null)
            {
                IEventSagaReceiveResponseUpdated newMessage = new EventSagaReceiveResponseUpdated()
                {
                    Response = result
                };
                await _serviceBus.BusAccess.Publish(newMessage);
            }

            return Ok(result);
        }
    }
}
