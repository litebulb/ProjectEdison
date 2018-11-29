using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Edison.Core.Interfaces;
using Edison.ChatService.Config;
using Edison.ChatService.Models;


namespace Edison.ChatService.Controllers
{
    /// <summary>
    /// Controller to handle security related to the Bot Chat
    /// </summary>
    [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C)]
    [Route("Security")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly BotOptions _config;
        private static IDirectLineRestService _directLineRestClient;
        private ILogger<SecurityController> _logger;

        /// <summary>
        /// DI constructor
        /// </summary>
        public SecurityController(IOptions<BotOptions> config, IDirectLineRestService directLineRestClient, 
            ILogger<SecurityController> logger)
        {
            _config = config.Value;
            _directLineRestClient = directLineRestClient;
            _logger = logger;
        }

        /// <summary>
        /// Retrieve a token using the SECRET of the direct line bot.
        /// This call is made to be used with advanced security. Therefore user ids will start with "dl_"
        /// </summary>
        /// <returns>ChatUserTokenContext</returns>
        [HttpGet("GetToken")]
        public async Task<IActionResult> GetToken()
        {
            try
            {
                ChatUserContext userContext = ChatUserContext.FromClaims(User.Claims, _config);
                TokenConversationResult conversation = await _directLineRestClient.GenerateToken(new TokenConversationParameters()
                {  
                    User = userContext
                });
                if(conversation == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
                return Ok(new ChatUserTokenContext()
                {
                    ConversationId = conversation.ConversationId,
                    Token = conversation.Token,
                    ExpiresIn = conversation.ExpiresIn,
                    UserContext = userContext
                });
            }
            catch(Exception e)
            {
                _logger.LogError($"SecurityController: {e.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
        
    }
}
