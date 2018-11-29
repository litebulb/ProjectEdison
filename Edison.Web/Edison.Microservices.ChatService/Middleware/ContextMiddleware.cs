using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Connector;
using Edison.Core.Common.Models;
using Edison.ChatService.Config;
using Edison.ChatService.Helpers;
using Edison.ChatService.Models;

namespace Edison.ChatService.Middleware
{
    /// <summary>
    /// Middleware that retrieves the user context and role.
    /// </summary>
    public class ContextMiddleware : IMiddleware
    {
        private readonly BotOptions _config;
        private readonly BotRoutingDataManager _routingDataManager;
        private readonly ILogger<ContextMiddleware> _logger;

        public ContextMiddleware(IOptions<BotOptions> config,
            BotRoutingDataManager routingDataManager,
            ILogger<ContextMiddleware> logger)
        {
            _config = config.Value;
            _routingDataManager = routingDataManager;
            _logger = logger;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            Activity activity = turnContext.Activity;

            if (activity.Type is ActivityTypes.Message)
            {
                //Add User Context in the turn context
                ConversationReference selfConversation = activity.GetConversationReference();
                ChatUserContext userContext = ChatUserContext.FromConversation(selfConversation);
                if (activity.ChannelId.ToLower() != _config.AdminChannel || 
                    !UserRoleCache.UserRoles.ContainsKey(userContext.Id))
                    userContext.Role = ChatUserRole.Consumer;
                else
                    userContext.Role = UserRoleCache.UserRoles[userContext.Id];
                selfConversation.User.Role = userContext.Role.ToString();
                turnContext.TurnState.Add(typeof(ChatUserContext).FullName, userContext);

                //Add Message router in the turn context
                ConnectorClient connectorClient = turnContext.TurnState.Get<ConnectorClient>(typeof(IConnectorClient).FullName);
                MessageRouter messageRouter = new MessageRouter(connectorClient, activity, _logger);
                turnContext.TurnState.Add(typeof(MessageRouter).FullName, messageRouter);

                //Ensure that each user only has one conversation active, and notify the other sessions where a conversation replaced it.
                IEnumerable<ConversationReference> otherConversations = await _routingDataManager.GetConversationsFromUser(selfConversation.User.Id);
                //Remove current conversation
                otherConversations = _routingDataManager.RemoveSelfConversation(otherConversations, selfConversation);
                //Send a notification to all old conversations
                if (otherConversations.Count() > 0)
                    foreach(var otherConversation in otherConversations)
                        await messageRouter.SendErrorMessageAsync(otherConversation, "You were disconnected from this instance.");
                //Store the new conversation
                await _routingDataManager.SaveConversationReference(selfConversation);

                //Leave this middleware
                await next(cancellationToken).ConfigureAwait(false);
            }
            if (activity.Type is ActivityTypes.EndOfConversation)
            {
                ConversationReference selfConversation = activity.GetConversationReference();
                _logger.LogInformation($"Conversation Ended: {selfConversation.Conversation.Id}");
            }
        }
    }
}
