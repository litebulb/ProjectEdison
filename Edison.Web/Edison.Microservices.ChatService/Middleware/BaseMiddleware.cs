using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Bot.Builder;
using Edison.ChatService.Helpers;
using Edison.ChatService.Models;

namespace Edison.ChatService.Middleware
{
    /// <summary>
    /// Base Middleware that automatically adds the objects ChatUserContext and MessageRouter to a middleware
    /// </summary>
    public abstract class BaseMiddleware : IMiddleware
    {
        private readonly ILogger<BaseMiddleware> _logger;

        public BaseMiddleware(ILogger<BaseMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(turnContext.TurnState.TryGetValue(typeof(ChatUserContext).FullName, out object userContext) &&
               turnContext.TurnState.TryGetValue(typeof(MessageRouter).FullName, out object messageRouter))
            {
                await HandleTurnAsync(turnContext, (ChatUserContext)userContext, (MessageRouter)messageRouter, next, cancellationToken);
            }
            else
            {
                _logger.LogError("Error while retrieving UserContext or MessageRouter");
            }
        }

        protected string GetDatabaseUserId(string channelId, string userId)
        {
            if (channelId == "directline" && userId.StartsWith("dl_"))
                return userId.Substring(3);
            return userId;
        }

        protected abstract Task HandleTurnAsync(ITurnContext turnContext, ChatUserContext userContext, MessageRouter messageRouter, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken));
    }
}
