using Edison.ChatService.Config;
using Edison.ChatService.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Edison.ChatService.Models;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using System.Net;
using Edison.Core.Interfaces;
using Edison.Core.Common.Models;

namespace Edison.ChatService.Controllers
{
    [Authorize(AuthenticationSchemes = "B2CWeb,AzureAd")]
    [Route("Security")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly BotOptions _config;
        private static IDirectLineRestService _directLineRestClient;
        private ILogger<SecurityController> _logger;

        public SecurityController(IOptions<BotOptions> config, IDirectLineRestService directLineRestClient, 
            ILogger<SecurityController> logger)
        {
            _config = config.Value;
            _directLineRestClient = directLineRestClient;
            _logger = logger;
        }

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
