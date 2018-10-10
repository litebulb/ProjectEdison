using AutoMapper;
using Edison.Core.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Edison.Api.Controllers
{
    [Authorize(AuthenticationSchemes = "Backend,B2CWeb")]
    [Route("api/SignalR")]
    [ApiController]
    public class SignalRController : ControllerBase
    {
        private readonly IHubContext<SignalRHub> _hub;
        private readonly IMapper _Mapper;

        public SignalRController(IMapper mapper, IHubContext<SignalRHub> hub)
        {
            _Mapper = mapper;
            _hub = hub;
        }

        [HttpPut("EventCluster")]
        public async Task<IActionResult> UpdateEventClusterUI([FromBody]EventClusterUIModel eventClusterUIUpdate)
        {
            await _hub.Clients.All.SendAsync("UpdateEventClusterUI", eventClusterUIUpdate);
            return Ok();
        }

        [HttpPut("Device")]
        public async Task<IActionResult> UpdateDeviceUI([FromBody]DeviceUIModel deviceUIUpdate)
        {
            await _hub.Clients.All.SendAsync("UpdateDeviceUI", deviceUIUpdate);
            return Ok();
        }

        [HttpPut("Response")]
        public async Task<IActionResult> UpdateResponseUI([FromBody]ResponseUIModel responseUIUpdate)
        {
            await _hub.Clients.All.SendAsync("UpdateResponseUI", responseUIUpdate);
            return Ok();
        }
    }
}
