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
using Twilio.TwiML;
using Twilio.TwiML.Voice;
using Twilio;
using System.Text;

namespace Edison.Api.Controllers
{
    [ApiController]
    [Route("api/Twilio")]
    public class TwilioController : Controller
    {
        private static IConfiguration _configuration;
        //private static TwilioRestClient _restClient;
        private static IOptions<TwilioOptions> _twilioOptions;

        public TwilioController(IConfiguration configuration, IOptions<TwilioOptions> twilioOptions)
        {
            _configuration = configuration;
            _twilioOptions = twilioOptions;
            TwilioClient.Init(_twilioOptions.Value.AccountSID, _twilioOptions.Value.AuthToken);
            //var twilioCreator = new ProxiedTwilioClientCreator(_twilioOptions);
            //_restClient = twilioCreator.GetClient();
        }

        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureAD, Policy = AuthenticationRoles.Admin)]
        [Route("Emergency")]
        [Produces(typeof(TwilioModel))]
        [HttpPost]
        public async Task<IActionResult> EmergencyCall(TwilioModel obj)
        {
            try
            {
                var to = new PhoneNumber(string.Concat("+",_twilioOptions.Value.EmergencyPhoneNumber));
                var from = new PhoneNumber(string.Concat("+", _twilioOptions.Value.PhoneNumber));
                var result = new TwilioModel();
                if (_twilioOptions.Value.BypassCalling == "1" || _twilioOptions.Value.BypassCalling.ToLower() == "true")
                {
                    result.CallSID = Guid.Empty.ToString();
                    result.Message = "Call Bypassed";
                }
                else
                {
                    var call = await CallResource.CreateAsync(to, from, url: new Uri(_twilioOptions.Value.ProxyServerUrl));

                    result = new TwilioModel()
                    {
                        CallSID = call.Sid,
                        Message = obj.Message
                    };
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status502BadGateway, e.Message);
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateTwilioRequestAttribute))]
        [Route("Interconnect")]
        [Produces("text/xml")]
        public IActionResult Interconnect()
        {
            try
            {
                var response = new VoiceResponse();
                var dial = new Dial();
                dial.Number(string.Concat("+", _twilioOptions.Value.CallForwardingPhoneNumber), sendDigits: "wwww1928");
                response.Append(dial);

                return Content(response.ToString(), "text/xml");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status502BadGateway, e.Message);
            }
        }
    }
}
