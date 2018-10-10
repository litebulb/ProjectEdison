using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edison.Api.Helpers;
using Edison.Common.Interfaces;
using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edison.Api.Controllers
{
    [Authorize(AuthenticationSchemes = "Backend,B2CWeb")]
    [ApiController]
    [Route("api/Responses")]
    public class ResponsesController : ControllerBase
    {
        private readonly ResponseDataManager _responseDataManager;
        private readonly IServiceBusClient _serviceBus;

        public ResponsesController(ResponseDataManager responseDataManager, IServiceBusClient serviceBusClient)
        {
            _responseDataManager = responseDataManager;
            _serviceBus = serviceBusClient;
        }
 
        [HttpGet("{responseId}")]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> GetResponseDetail(Guid responseId)
        {
            ResponseModel responseObj = await _responseDataManager.GetResponse(responseId);
            return Ok(responseObj);
        }

        [HttpGet]
        [Produces(typeof(IEnumerable<ResponseLightModel>))]
        public async Task<IActionResult> GetResponses()
        {
            IEnumerable<ResponseLightModel> responseObjs = await _responseDataManager.GetResponses();
            return Ok(responseObjs);
        }

        [HttpPost("Radius")]
        [Produces(typeof(IEnumerable<ResponseModel>))]
        public async Task<IActionResult> GetResponsesFromPointRadius([FromBody]ResponseGeolocationModel responseGeolocationObj)
        {
            IEnumerable<ResponseModel> responseObjs = await _responseDataManager.GetResponsesFromPointRadius(responseGeolocationObj);
            return Ok(responseObjs);
        }

        [HttpPost]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> CreateResponse([FromBody]ResponseCreationModel responseObj)
        {
            var result = await _responseDataManager.CreateResponse(responseObj);
            IEventSagaReceiveResponseCreated newMessage = new EventSagaReceiveResponseCreated()
            {
                ResponseModel = result
            };
            await _serviceBus.BusAccess.Publish(newMessage);
            return Ok(result);
        }

        [HttpPut]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> UpdateResponse(ResponseUpdateModel responseObj)
        {
            var result = await _responseDataManager.UpdateResponse(responseObj);
            IEventSagaReceiveResponseUpdated newMessage = new EventSagaReceiveResponseUpdated()
            {
                ResponseModel = result
            };
            await _serviceBus.BusAccess.Publish(newMessage);
            return Ok(result);
        }

        [HttpPut("Close")]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> CloseResponse(ResponseCloseModel responseObj)
        {
            var result = await _responseDataManager.CloseResponse(responseObj);
            IEventSagaReceiveResponseUpdated newMessage = new EventSagaReceiveResponseUpdated()
            {
                ResponseModel = result
            };
            await _serviceBus.BusAccess.Publish(newMessage);
            return Ok(result);
        }

        [HttpPut("AddEventClusters")]
        [Produces(typeof(ResponseModel))]
        public async Task<IActionResult> AddEventClusterIdsToResponse(ResponseEventClustersUpdateModel responseObj)
        {
            ResponseModel result = await _responseDataManager.AddEventClusterIdsToResponse(responseObj);
            return Ok(result);
        }

        [HttpDelete]
        [Produces(typeof(bool))]
        public async Task<IActionResult> DeleteResponse(Guid responseId)
        {
            var result = await _responseDataManager.DeleteResponse(responseId);
            return Ok(result);
        }
    }
}
