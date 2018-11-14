using Edison.ChatService.Config;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using Edison.ChatService.Models;
using Edison.ChatService.Helpers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Connector;
using Edison.Core.Common.Models;
using Edison.ChatService.Helpers.Interfaces;
using System;

namespace Edison.ChatService.Middleware
{
    public class ContextMiddleware : Microsoft.Bot.Builder.IMiddleware
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
