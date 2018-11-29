using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Edison.Api.Helpers;

namespace Edison.Api.Controllers
{
    /// <summary>
    /// Controller to handle operations on signalr
    /// </summary>
    [ApiController]
    [Route("api/SignalR")]
    public class SignalRController : ControllerBase
    {
        private readonly IHubContext<SignalRHub> _hub;
        private readonly ResponseDataManager _responseDataManager;

        public SignalRController(IHubContext<SignalRHub> hub, ResponseDataManager responseDataManager)
        {
            _hub = hub;
            _responseDataManager = responseDataManager;
        }

        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpPut("EventCluster")]
        public async Task<IActionResult> UpdateEventClusterUI([FromBody]EventClusterUIModel eventClusterUIUpdate)
        {
            await _hub.Clients.All.SendAsync("UpdateEventClusterUI", eventClusterUIUpdate);
            return Ok();
        }

        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpPut("Device")]
        public async Task<IActionResult> UpdateDeviceUI([FromBody]DeviceUIModel deviceUIUpdate)
        {
            await _hub.Clients.All.SendAsync("UpdateDeviceUI", deviceUIUpdate);
            return Ok();
        }

        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpPut("Response")]
        public async Task<IActionResult> UpdateResponseUI([FromBody]ResponseUIModel responseUIUpdate)
        {
            if (responseUIUpdate.Response == null && responseUIUpdate.ResponseId != Guid.Empty)
                responseUIUpdate.Response = await _responseDataManager.GetResponse(responseUIUpdate.ResponseId);
            await _hub.Clients.All.SendAsync("UpdateResponseUI", responseUIUpdate);
            return Ok();
        }

        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.SuperAdmin)]
        [HttpPut("Response/ActionClose")]
        public async Task<IActionResult> UpdateActionCallbackUI([FromBody]ActionCallbackUIModel actionCallbackUIUpdate)
        {
            await _hub.Clients.All.SendAsync("UpdateActionCallbackUI", actionCallbackUIUpdate);
            return Ok();
        }
    }
}
