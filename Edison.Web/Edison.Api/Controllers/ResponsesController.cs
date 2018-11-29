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

        public ResponsesController(ResponseDataManager responseDataManager,
            IOptions<WebApiOptions> config, IMassTransitServiceBus serviceBusClient)
        {
            _config = config.Value;
            _responseDataManager = responseDataManager;
            _serviceBus = serviceBusClient;
        }

        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Consumer)]
        [HttpGet("{responseId}")]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> GetResponseDetail(Guid responseId)
        {
            ResponseModel responseObj = await _responseDataManager.GetResponse(responseId);
            return Ok(responseObj);
        }

        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Consumer)]
        [HttpGet]
        [Produces(typeof(IEnumerable<ResponseLightModel>))]
        public async Task<IActionResult> GetResponses()
        {
            IEnumerable<ResponseLightModel> responseObjs = await _responseDataManager.GetResponses();
            return Ok(responseObjs);
        }

        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Consumer)]
        [HttpPost("Radius")]
        [Produces(typeof(IEnumerable<ResponseModel>))]
        public async Task<IActionResult> GetResponsesFromPointRadius([FromBody]ResponseGeolocationModel responseGeolocationObj)
        {
            IEnumerable<ResponseModel> responseObjs = await _responseDataManager.GetResponsesFromPointRadius(responseGeolocationObj);
            return Ok(responseObjs);
        }

        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Consumer)]
        [HttpPut("Safe")]
        [Produces(typeof(bool))]
        public async Task<IActionResult> SetSafeStatus([FromBody]ResponseSafeUpdateModel responseSafeUpdateObj)
        {
            string userId = UserHelper.GetBestClaimValue(User.Claims.ToList(), _config.ClaimsId, true).ToLower();

            bool result = await _responseDataManager.SetSafeStatus(userId, responseSafeUpdateObj.IsSafe);
            return Ok(result);
        }

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

        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpPut("AddEventClusters")]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> AddEventClusterIdsToResponse(ResponseEventClustersUpdateModel responseObj)
        {
            ResponseModel result = await _responseDataManager.AddEventClusterIdsToResponse(responseObj);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpDelete]
        [Produces(typeof(bool))]
        public async Task<IActionResult> DeleteResponse(Guid responseId)
        {
            var result = await _responseDataManager.DeleteResponse(responseId);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpPost("CompleteAction")]
        [Produces(typeof(bool))]
        public async Task<IActionResult> CompleteAction(ActionCompletionModel actionCompletionObj)
        {
            var result = await _responseDataManager.CompleteAction(actionCompletionObj);
            return Ok(result);
        }

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
