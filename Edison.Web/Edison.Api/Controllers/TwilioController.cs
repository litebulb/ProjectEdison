using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Edison.Api.Config;
using Edison.Api.Helpers;

namespace Edison.Api.Controllers
{
    [ApiController]
    [Route("api/Twilio")]
    public class TwilioController : Controller
    {
        private static IConfiguration _configuration;
        private static TwilioRestClient _restClient;
        private static IOptions<TwilioOptions> _twilioOptions;

        public TwilioController(IConfiguration configuration, IOptions<TwilioOptions> twilioOptions)
        {
            _configuration = configuration;
            _twilioOptions = twilioOptions;
            var twilioCreator = new ProxiedTwilioClientCreator(_twilioOptions);
            _restClient = twilioCreator.GetClient();
        }

        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [Route("Emergency")]
        [Produces(typeof(DeviceMobileModel))]
        [HttpGet]
        public async Task<IActionResult> EmergencyCall()
        {
            try
            {
                var to = new PhoneNumber(string.Concat("+",_twilioOptions.Value.EmergencyPhoneNumber));
                var from = new PhoneNumber(string.Concat("+", _twilioOptions.Value.PhoneNumber));
                var call = CallResource.Create(to, from,
                    url: new Uri("http://demo.twilio.com/docs/voice.xml"));

                Console.WriteLine(call.Sid);
                await new Task(() => { ; });
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status502BadGateway, e.Message);
            }
        }
    }
}
