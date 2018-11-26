using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edison.Api.Config;
using Edison.Api.Helpers;
using Edison.Common.Interfaces;
using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Edison.Api.Controllers
{
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

        [Authorize(AuthenticationSchemes = "AzureAd,B2CWeb", Policy = "Consumer")]
        [HttpGet("{responseId}")]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> GetResponseDetail(Guid responseId)
        {
            ResponseModel responseObj = await _responseDataManager.GetResponse(responseId);
            return Ok(responseObj);
        }

        [Authorize(AuthenticationSchemes = "AzureAd,B2CWeb", Policy = "Consumer")]
        [HttpGet]
        [Produces(typeof(IEnumerable<ResponseLightModel>))]
        public async Task<IActionResult> GetResponses()
        {
            IEnumerable<ResponseLightModel> responseObjs = await _responseDataManager.GetResponses();
            return Ok(responseObjs);
        }

        [Authorize(AuthenticationSchemes = "AzureAd,B2CWeb", Policy = "Consumer")]
        [HttpPost("Radius")]
        [Produces(typeof(IEnumerable<ResponseModel>))]
        public async Task<IActionResult> GetResponsesFromPointRadius([FromBody]ResponseGeolocationModel responseGeolocationObj)
        {
            IEnumerable<ResponseModel> responseObjs = await _responseDataManager.GetResponsesFromPointRadius(responseGeolocationObj);
            return Ok(responseObjs);
        }

        [Authorize(AuthenticationSchemes = "AzureAd,B2CWeb", Policy = "Consumer")]
        [HttpPut("Safe")]
        [Produces(typeof(bool))]
        public async Task<IActionResult> SetSafeStatus([FromBody]ResponseSafeUpdateModel responseSafeUpdateObj)
        {
            string userId = UserHelper.GetBestClaimValue(User.Claims.ToList(), _config.ClaimsId, true).ToLower();

            bool result = await _responseDataManager.SetSafeStatus(userId, responseSafeUpdateObj.IsSafe);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpPost]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> CreateResponse([FromBody]ResponseCreationModel responseObj)
        {
            var result = await _responseDataManager.CreateResponse(responseObj);
            if (!responseObj.DelayStart)
            {
                IEventSagaReceiveResponseCreated newMessage = new EventSagaReceiveResponseCreated()
                {
                    ResponseModel = result
                };
                await _serviceBus.BusAccess.Publish(newMessage);
            }
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpPost("Locate")]
        public async Task<IActionResult> LocateResponse([FromBody]ResponseStartModel responseObj)
        {
            var result = await _responseDataManager.LocateResponse(new ResponseUpdateModel()
            {
                ResponseId = responseObj.ResponseId,
                Geolocation = responseObj.Geolocation
            }
            );
            IEventSagaReceiveResponseCreated newMessage = new EventSagaReceiveResponseCreated()
            {
                ResponseModel = result
            };
            await _serviceBus.BusAccess.Publish(newMessage);
            return Ok();
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpPut("Close")]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> CloseResponse(ResponseCloseModel responseObj)
        {
            var result = await _responseDataManager.CloseResponse(responseObj);
            IEventSagaReceiveResponseClosed newMessage = new EventSagaReceiveResponseClosed()
            {
                ResponseModel = result
            };
            await _serviceBus.BusAccess.Publish(newMessage);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "SuperAdmin")]
        [HttpPut("AddEventClusters")]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> AddEventClusterIdsToResponse(ResponseEventClustersUpdateModel responseObj)
        {
            ResponseModel result = await _responseDataManager.AddEventClusterIdsToResponse(responseObj);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "SuperAdmin")]
        [HttpDelete]
        [Produces(typeof(bool))]
        public async Task<IActionResult> DeleteResponse(Guid responseId)
        {
            var result = await _responseDataManager.DeleteResponse(responseId);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "Admin")]
        [HttpPut("ChangeAction")]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> AddActionToResponse(ResponseChangeActionPlanModel responseObj)
        {
            ResponseModel result = await _responseDataManager.ChangeActionOnResponse(responseObj);
            if (result != null)
            {
                EventSagaReceiveResponseActionsUpdated newMessage = new EventSagaReceiveResponseActionsUpdated()
                {
                    //We do not want to trigger events for deleted actions or close actions
                    Actions = responseObj.Actions.Where(x => !x.IsCloseAction && x.ActionChangedString != "delete").Select(a => a.Action),
                    ResponseId = responseObj.ResponseId,
                    Geolocation = result.Geolocation,
                    PrimaryRadius = result.ActionPlan.PrimaryRadius,
                    SecondaryRadius = result.ActionPlan.SecondaryRadius
                };
                await _serviceBus.BusAccess.Publish(newMessage);
            }

            return Ok(result);
        }
    }
}
