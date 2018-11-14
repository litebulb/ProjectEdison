using AutoMapper;
using Edison.Core.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Edison.Api.Controllers
{
    [ApiController]
    [Route("api/SignalR")]
    public class SignalRController : ControllerBase
    {
        private readonly IHubContext<SignalRHub> _hub;
        private readonly IMapper _Mapper;

        public SignalRController(IMapper mapper, IHubContext<SignalRHub> hub)
        {
            _Mapper = mapper;
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
        public async Task<IActionResult> UpdateActionCloseUI([FromBody]ActionCloseUIModel actionCloseUIUpdate)
        {
            await _hub.Clients.All.SendAsync("UpdateActionCloseUI", actionCloseUIUpdate);
            return Ok();
        }
    }
}
