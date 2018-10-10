using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Edison.Common.Chat.Models.Interface;
using Edison.Common.Chat.Models;
using Edison.Common.Chat.Config;

namespace Edison.Common.Chat.Repositories
{
    public class MessageRouterManager
    {
        public IChatRoutingDataManagerRepository _routingDataManager { get;set;}
        private static IConversationChatBot _conversationChatBot;
        private static MicrosoftAppCredentials _credentials;
        public static IOptions<BotOptions> _botOptions;

        public MessageRouterManager(IChatRoutingDataManagerRepository routingDataManager, IOptions<BotOptions> botOptions,
            IConversationChatBot conversationChatBot)
        {
            _routingDataManager = routingDataManager;
            _botOptions = botOptions;
            _conversationChatBot = conversationChatBot;
            _credentials = new MicrosoftAppCredentials()
            {
                MicrosoftAppId = botOptions.Value.MicrosoftAppId,
                MicrosoftAppPassword = botOptions.Value.MicrosoftAppPassword
            };
        }

        public virtual async Task<MessageRouterResult> HandleActivityAsync(
            Activity activity,
            bool tryToRequestConnectionIfNotConnected,
            bool rejectConnectionRequestIfNoAggregationChannel,
            bool addClientNameToMessage = true,
            bool addOwnerNameToMessage = false)
        {
            MakeSurePartiesAreTracked(activity);

            MessageRouterResult messageRouterResult =
                await RouteMessageIfSenderIsConnectedAsync(activity, addClientNameToMessage, addOwnerNameToMessage);

            if (tryToRequestConnectionIfNotConnected
                && messageRouterResult.Type == MessageRouterResultType.NoActionTaken)
            {
                messageRouterResult = RequestConnection(activity, rejectConnectionRequestIfNoAggregationChannel);
            }

            return messageRouterResult;
        }


        public async Task<string> SendMessageToPartyByBotAsync(
            Party partyToMessage, IMessageActivity messageActivity)
        {
            Party botParty = null;

            if (partyToMessage != null)
            {
                botParty = _routingDataManager.FindBotPartyByChannelAndConversation(
                    partyToMessage.ChannelId, partyToMessage.ConversationAccount);
            }

            if (botParty != null)
            {
                messageActivity.From = botParty.ChannelAccount;
                messageActivity.Recipient = partyToMessage.ChannelAccount;

                var connector = _conversationChatBot.Create(new Uri(partyToMessage.ServiceUrl), _credentials);
                MicrosoftAppCredentials.TrustServiceUrl(partyToMessage.ServiceUrl, DateTime.Now.AddDays(7));

                var response = await _conversationChatBot.SendMessageToConversations(connector, (Activity)messageActivity);
                if (response != null)
                {
                    return response;
                }
            }

            return null;
        }

        public async Task<string> SendMessageToPartyByBotAsync(Party partyToMessage, string messageText)
        {
            Party botParty = null;

            if (partyToMessage != null)
            {
                botParty = _routingDataManager.FindBotPartyByChannelAndConversation(
                    partyToMessage.ChannelId, partyToMessage.ConversationAccount);
            }

            if (botParty != null)
            {
                IMessageActivity activity =
                   MessagingUtils.CreateMessageActivity(
                       partyToMessage, messageText, botParty?.ChannelAccount, _botOptions);
                var connector=_conversationChatBot.Create(new Uri(partyToMessage.ServiceUrl), _credentials);
                MicrosoftAppCredentials.TrustServiceUrl(partyToMessage.ServiceUrl, DateTime.Now.AddDays(7));

                var response = await _conversationChatBot.SendMessageToConversations(connector,(Activity) activity);
                if (response != null)
                {
                    return response;
                }
            }

            return null;
        }

        public void MakeSurePartiesAreTracked(Party senderParty, Party recipientParty)
        {
            // Store the bot identity, if not already stored
            _routingDataManager.AddParty(recipientParty, false);

            var botParties = _routingDataManager.GetBotParties();
            if(botParties!=null)
            {
                if (!botParties.Contains(senderParty))
                {
                    // Store the user party, if not already stored
                    _routingDataManager.AddParty(senderParty);
                }
            }
            // Check that the party who sent the message is not the bot          
        }

        public void MakeSurePartiesAreTracked(Activity activity)
        {
            MakeSurePartiesAreTracked(
                MessagingUtils.CreateSenderParty(activity),
                MessagingUtils.CreateRecipientParty(activity));
        }

       
        public IList<MessageRouterResult> RemoveParty(Party partyToRemove)
        {
            return _routingDataManager.RemoveParty(partyToRemove);
        }

        public MessageRouterResult RequestConnection(
            Party requestorParty, bool rejectConnectionRequestIfNoAggregationChannel = false)
        {
            return _routingDataManager.AddPendingRequest(requestorParty, rejectConnectionRequestIfNoAggregationChannel);
        }

        public virtual MessageRouterResult RequestConnection(
            Activity activity, bool rejectConnectionRequestIfNoAggregationChannel = false)
        {
            MessageRouterResult messageRouterResult =
                RequestConnection(MessagingUtils.CreateSenderParty(activity), rejectConnectionRequestIfNoAggregationChannel);
            if (messageRouterResult != null)
            {
                messageRouterResult.Activity = activity;
            }       
            return messageRouterResult;
        }

        public virtual MessageRouterResult RemovePendingRequest(
            Party partyToRemove)
        {
            if (partyToRemove == null)
            {
                throw new ArgumentNullException($"The party to remove ({nameof(partyToRemove)} cannot be null");
            }

            MessageRouterResult messageRouteResult = _routingDataManager.RemovePendingRequest(partyToRemove);
            return messageRouteResult;
        }

        public virtual MessageRouterResult RejectPendingRequest(Party partyToReject, Party rejecterParty = null)
        {
            if (partyToReject == null)
            {
                throw new ArgumentNullException($"The party to reject ({nameof(partyToReject)} cannot be null");
            }

            MessageRouterResult messageRouteResult = _routingDataManager.RemovePendingRequest(partyToReject);
            messageRouteResult.ConversationClientParty = partyToReject;
            messageRouteResult.ConversationAgentParty = rejecterParty;

            if (messageRouteResult.Type == MessageRouterResultType.Error)
            {
                messageRouteResult.ErrorMessage =
                    $"Failed to remove the pending request of user \"{partyToReject.ChannelAccount?.Name}\": {messageRouteResult.ErrorMessage}";
            }

            return messageRouteResult;
        }
       
        public virtual async Task<MessageRouterResult> ConnectAsync(
            Party conversationAgentParty, Party conversationClientParty, bool createNewDirectConversation)
        {
            if (conversationAgentParty == null || conversationClientParty == null)
            {
                throw new ArgumentNullException(
                    $"Neither of the arguments ({nameof(conversationAgentParty)}, {nameof(conversationClientParty)}) can be null");
            }

            MessageRouterResult result = new MessageRouterResult()
            {
                ConversationAgentParty = conversationAgentParty,
                ConversationClientParty = conversationClientParty
            };

            Party botParty = _routingDataManager.FindBotPartyByChannelAndConversation(
                conversationAgentParty.ChannelId, conversationAgentParty.ConversationAccount);

            if (botParty != null)
            {
                if (createNewDirectConversation)
                {
                    ConnectorClient connectorClient = new ConnectorClient(new Uri(conversationAgentParty.ServiceUrl));
                    ConversationResourceResponse conversationResourceResponse = null;

                    try
                    {
                        conversationResourceResponse =
                            await connectorClient.Conversations.CreateDirectConversationAsync(
                                botParty.ChannelAccount, conversationAgentParty.ChannelAccount);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to create a direct conversation: {e.Message}");
                        // Do nothing here as we fallback (continue without creating a direct conversation)
                    }

                    if (conversationResourceResponse != null && !string.IsNullOrEmpty(conversationResourceResponse.Id))
                    {
                        // The conversation account of the conversation owner for this 1:1 chat is different -
                        // thus, we need to re-create the conversation owner instance
                        ConversationAccount directConversationAccount =
                            new ConversationAccount(id: conversationResourceResponse.Id);

                        conversationAgentParty = new Party(
                            conversationAgentParty.ServiceUrl, conversationAgentParty.ChannelId,
                            conversationAgentParty.ChannelAccount, directConversationAccount);

                        _routingDataManager.AddParty(conversationAgentParty);
                        _routingDataManager.AddParty(new Party(
                            botParty.ServiceUrl, botParty.ChannelId, botParty.ChannelAccount, directConversationAccount), false);

                        result.ConversationResourceResponse = conversationResourceResponse;
                    }
                }

                result = _routingDataManager.ConnectAndClearPendingRequestAsync(conversationAgentParty, conversationClientParty);
            }
            else
            {
                result.Type = MessageRouterResultType.Error;
                result.ErrorMessage = "Failed to find the bot instance";
            }

            return result;
        }

        public List<MessageRouterResult> Disconnect(Party connectedParty)
        {
            return Disconnect(connectedParty, ConnectionProfileType.Any);
        }

        public MessageRouterResult DisconnectClient(Party conversationClientParty)
        {
            // There can be only one result since a client cannot be connected in multiple conversations
            return Disconnect(conversationClientParty, ConnectionProfileType.Client)[0];
        }

        public List<MessageRouterResult> DisconnectOwner(Party conversationOwnerParty)
        {
            return Disconnect(conversationOwnerParty, ConnectionProfileType.Agent);
        }

        public virtual async Task<MessageRouterResult> RouteMessageIfSenderIsConnectedAsync(
            Activity activity, bool addClientNameToMessage = true, bool addOwnerNameToMessage = false)
        {
            MessageRouterResult result = new MessageRouterResult()
            {
                Type = MessageRouterResultType.NoActionTaken,
                Activity = activity
            };

            Party senderParty = MessagingUtils.CreateSenderParty(activity);

            if (_routingDataManager.IsConnected(senderParty, ConnectionProfileType.Agent))
            {
                // Sender is an owner of an ongoing conversation - forward the message
                result.ConversationAgentParty = senderParty;
                Party partyToForwardMessageTo = _routingDataManager.GetConnectedCounterpart(senderParty);

                if (partyToForwardMessageTo != null)
                {
                    result.ConversationClientParty = partyToForwardMessageTo;
                    string message = addOwnerNameToMessage
                        ? $"{senderParty.ChannelAccount.Name}: {activity.Text}" : activity.Text;
                    var resourceResponse =
                        await SendMessageToPartyByBotAsync(partyToForwardMessageTo, activity.Text);

                    if (resourceResponse != null)
                    {
                        result.Type = MessageRouterResultType.OK;
                    }
                    else
                    {
                        result.Type = MessageRouterResultType.FailedToForwardMessage;
                        result.ErrorMessage = $"Failed to forward the message to user {partyToForwardMessageTo}";
                    }
                }
                else
                {
                    result.Type = MessageRouterResultType.FailedToForwardMessage;
                    result.ErrorMessage = "Failed to find the party to forward the message to";
                }
            }
            else if (_routingDataManager.IsConnected(senderParty, ConnectionProfileType.Client))
            {
                // Sender is a participant of an ongoing conversation - forward the message
                result.ConversationClientParty = senderParty;
                Party partyToForwardMessageTo = _routingDataManager.GetConnectedCounterpart(senderParty);

                if (partyToForwardMessageTo != null)
                {
                    result.ConversationAgentParty = partyToForwardMessageTo;
                    //string message = addClientNameToMessage
                    //    ? $"{senderParty.ChannelAccount.Name}: {activity.Text}" : activity.Text;
                    string message = activity.Text;
                    await SendMessageToPartyByBotAsync(partyToForwardMessageTo, message);
                    result.Type = MessageRouterResultType.OK;
                }
                else
                {
                    result.Type = MessageRouterResultType.FailedToForwardMessage;
                    result.ErrorMessage = "Failed to find the party to forward the message to";
                }
            }

            return result;
        }

        protected virtual async Task<ResourceResponse> SendAsync(
            MessagingUtils.ConnectorClientAndMessageBundle bundle)
        {
            ResourceResponse resourceResponse = null;

            try
            {         
                resourceResponse =
                    await bundle.connectorClient.Conversations.SendToConversationAsync(
                        (Activity)bundle.messageActivity);
               
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Chat bot : Exception in Send Async{0}", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Chat bot : Exception in Send Async{0}", e.Message);
            }

            return resourceResponse;
        }

        protected virtual List<MessageRouterResult> Disconnect(Party connectedParty, ConnectionProfileType connectionProfile)
        {
            List<MessageRouterResult> messageRouterResults = new List<MessageRouterResult>();

            Party partyInConversation = _routingDataManager.FindConnectedPartyByChannel(
                connectedParty.ChannelId, connectedParty.ChannelAccount);

            if (partyInConversation != null
                && _routingDataManager.IsConnected(partyInConversation, connectionProfile))
            {
                messageRouterResults.AddRange(
                    _routingDataManager.DisconnectAsync(partyInConversation, connectionProfile));
            }
            else
            {
                MessageRouterResult messageRouterResult = new MessageRouterResult()
                {
                    Type = MessageRouterResultType.Error,
                    ErrorMessage = "No connection to disconnect found"
                };

                if (connectionProfile == ConnectionProfileType.Client)
                {
                    messageRouterResult.ConversationClientParty = connectedParty;
                }
                else if (connectionProfile == ConnectionProfileType.Agent)
                {
                    messageRouterResult.ConversationAgentParty = connectedParty;
                }

                messageRouterResults.Add(messageRouterResult);
            }

            return messageRouterResults;
        }
    }
}
