using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Edison.Core.Common.Models;

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

        public SignalRController(IHubContext<SignalRHub> hub)
        {
            _hub = hub;
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "SuperAdmin")]
        [HttpPut("EventCluster")]
        public async Task<IActionResult> UpdateEventClusterUI([FromBody]EventClusterUIModel eventClusterUIUpdate)
        {
            await _hub.Clients.All.SendAsync("UpdateEventClusterUI", eventClusterUIUpdate);
            return Ok();
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "SuperAdmin")]
        [HttpPut("Device")]
        public async Task<IActionResult> UpdateDeviceUI([FromBody]DeviceUIModel deviceUIUpdate)
        {
            await _hub.Clients.All.SendAsync("UpdateDeviceUI", deviceUIUpdate);
            return Ok();
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "SuperAdmin")]
        [HttpPut("Response")]
        public async Task<IActionResult> UpdateResponseUI([FromBody]ResponseUIModel responseUIUpdate)
        {
            await _hub.Clients.All.SendAsync("UpdateResponseUI", responseUIUpdate);
            return Ok();
        }

        [Authorize(AuthenticationSchemes = "AzureAd", Policy = "SuperAdmin")]
        [HttpPut("Response/ActionClose")]
        public async Task<IActionResult> UpdateActionCallbackUI([FromBody]ActionCallbackUIModel actionCallbackUIUpdate)
        {
            await _hub.Clients.All.SendAsync("UpdateActionCallbackUI", actionCallbackUIUpdate);
            return Ok();
        }
    }
}
