using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Common.Chat.CommandHandling;
using Edison.Common.Chat.Models;
using Edison.Common.Chat.Models.Interface;
using Edison.Common.Chat.Repositories;
using Edison.Core.Common;

namespace Edison.Common.Chat.MessageRouting
{
    public class MessageRouterResultHandler
    {
        private MessageRouterManager _messageRouterManager;
        //private HubConnection _hubConnection;

        public MessageRouterResultHandler(MessageRouterManager messageRouterManager)
        {
            _messageRouterManager = messageRouterManager
                ?? throw new ArgumentNullException(
                    $"The message router manager ({nameof(messageRouterManager)}) cannot be null");
        }

        public virtual async Task HandleConnectionResultAsync(MessageRouterResult messageRouterResult)
        {
            try
            {
                if (messageRouterResult == null)
                {
                    throw new ArgumentNullException($"The given result ({nameof(messageRouterResult)}) is null");
                }

#if DEBUG
                _messageRouterManager._routingDataManager.AddMessageRouterResult(messageRouterResult);
#endif

                string message = string.Empty;

                switch (messageRouterResult.Type)
                {
                    case MessageRouterResultType.NoActionTaken:
                    case MessageRouterResultType.OK:
                        // No need to do anything
                        break;
                    case MessageRouterResultType.ConnectionRequested:
                    case MessageRouterResultType.ConnectionAlreadyRequested:
                    case MessageRouterResultType.ConnectionRejected:
                    case MessageRouterResultType.Connected:
                    case MessageRouterResultType.Disconnected:
                        await HandleConnectionChangedResultAsync(messageRouterResult);
                        break;
                    case MessageRouterResultType.NoAgentsAvailable:
                        await HandleNoAgentsAvailableResultAsync(messageRouterResult);
                        break;
                    case MessageRouterResultType.NoAggregationChannel:
                        await HandleNoAggregationChannelResultAsync(messageRouterResult);
                        break;
                    case MessageRouterResultType.FailedToForwardMessage:
                        await HandleFailedToForwardMessageAsync(messageRouterResult);
                        break;
                    case MessageRouterResultType.Error:
                        await HandleErrorAsync(messageRouterResult);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
               Console.WriteLine("Chat bot : Exception in HandleResultAsync{0}", e.Message);
            }
        }

        protected virtual async Task HandleErrorAsync(MessageRouterResult messageRouterResult)
        {
            string errorMessage = string.IsNullOrEmpty(messageRouterResult.ErrorMessage)
                ? string.Format($"An error occured (result: {0})", messageRouterResult.Type.ToString())
                : $"{messageRouterResult.ErrorMessage} ({messageRouterResult.Type})";

            System.Diagnostics.Debug.WriteLine(errorMessage);

            if (messageRouterResult.ConversationAgentParty != null)
            {
                await _messageRouterManager.SendMessageToPartyByBotAsync(
                    messageRouterResult.ConversationAgentParty, errorMessage);
            }
        }

        protected virtual async Task HandleFailedToForwardMessageAsync(MessageRouterResult messageRouterResult)
        {
            string messageText = string.IsNullOrEmpty(messageRouterResult.ErrorMessage)
                ? "Failed to forward the message"
                : messageRouterResult.ErrorMessage;
            await MessagingUtils.ReplyToActivityAsync(messageRouterResult.Activity, messageText);
            //await DebugActivityLogger.LogAsync(messageRouterResult.Activity, _eventHubRepo);
        }

        protected virtual async Task HandleNoAggregationChannelResultAsync(MessageRouterResult messageRouterResult)
        {
            if (messageRouterResult.Activity != null)
            {
                string messageText = string.IsNullOrEmpty(messageRouterResult.ErrorMessage)
                    ? string.Format("No aggregation channel set up")
                    : messageRouterResult.ErrorMessage;
                messageText += $" - ";
                messageText += string.Format(
                    $"To set up an aggregation channel, type '{0}'",
                    $"{Command.ResolveFullCommand(messageRouterResult.Activity.Recipient?.Name, Commands.CommandAddAggregationChannel)}");

                await MessagingUtils.ReplyToActivityAsync(messageRouterResult.Activity, messageText);
                //await DebugActivityLogger.LogAsync(messageRouterResult.Activity, _eventHubRepo);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("The activity of the result is null");
            }
        }

        protected virtual async Task HandleNoAgentsAvailableResultAsync(MessageRouterResult messageRouterResult)
        {
            if (messageRouterResult.Activity != null)
            {
                await MessagingUtils.ReplyToActivityAsync(messageRouterResult.Activity, "Sorry, there are no agents available right now");
                //await DebugActivityLogger.LogAsync(messageRouterResult.Activity, _eventHubRepo);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("The activity of the result is null");
            }
        }

        public virtual async Task<List<string>> HandleConnectionChangedResultAsync(MessageRouterResult messageRouterResult)
        {
            try
            {               
                var replyActivityList = new List<string>();
                IChatRoutingDataManagerRepository routingDataManager = _messageRouterManager._routingDataManager;

                Party conversationOwnerParty = messageRouterResult.ConversationAgentParty;
                Party conversationClientParty = messageRouterResult.ConversationClientParty;

                string conversationOwnerName =
                    string.IsNullOrEmpty(conversationOwnerParty?.ChannelAccount.Name)
                        ? StringAndCharConstants.NoUserNamePlaceholder
                        : conversationOwnerParty?.ChannelAccount.Name;

                string conversationClientName =
                    string.IsNullOrEmpty(conversationClientParty?.ChannelAccount.Name)
                        ? StringAndCharConstants.NoUserNamePlaceholder
                        : conversationClientParty?.ChannelAccount.Name;

                string messageToConversationOwner = string.Empty;
                string messageToConversationClient = string.Empty;

                if (messageRouterResult.Type == MessageRouterResultType.ConnectionRequested)
                {
                    bool conversationClientPartyMissing =
                        (conversationClientParty == null || conversationClientParty.ChannelAccount == null);

                    // send message request to SignalR Chat client
                    
                    //await _hubConnection.StartAsync();
                    //var messageRequest = String.Format("{0};{1}", conversationClientParty.ChannelAccount.Name, conversationClientParty.ConversationAccount.Id);
                    //await _hubConnection.InvokeAsync("MessageRequest", messageRequest);
                    //messageToConversationClient = ConversationText.NotifyClientWaitForRequestHandling;
                    //await _hubConnection.StopAsync();

                    //var agentParties = _messageRouterManager._routingDataManager.GetAgentParties();

                    //if (agentParties != null)
                    //{
                    //    if (agentParties.Count.Equals(0))
                    //    {
                    //        var result = _messageRouterManager.RemovePendingRequest(conversationClientParty);
                    //        messageToConversationClient = ConversationText.NotifyClientThatNoAgentIsAvailable;
                    //    }
                    //    else
                    //    {
                    //        foreach (Party aggregationParty in agentParties)
                    //        {
                    //            Party botParty = routingDataManager.FindBotPartyByChannelAndConversation(
                    //                aggregationParty.ChannelId, aggregationParty.ConversationAccount);

                    //            if (botParty != null)
                    //            {
                    //                if (conversationClientPartyMissing)
                    //                {
                    //                    var clientResponse = await _messageRouterManager.SendMessageToPartyByBotAsync(
                    //                        aggregationParty, ConversationText.RequestorDetailsMissing);
                    //                    replyActivityList.Add(clientResponse);
                    //                }
                    //                else
                    //                {
                    //                    ServiceEventSource.Current.Message("Chat bot : HandleConnectionChangedResultAsync Creating Request command card {0}", botParty.ToJsonString());

                    //                    IMessageActivity messageActivity = Activity.CreateMessageActivity();
                    //                    messageActivity.Conversation = aggregationParty.ConversationAccount;
                    //                    messageActivity.Recipient = aggregationParty.ChannelAccount;
                    //                    messageActivity.Attachments = new List<Attachment>
                    //                    {
                    //                        CommandCardFactory.CreateRequestCard(
                    //                            conversationClientParty, botParty.ChannelAccount?.Name).ToAttachment()
                    //                    };

                    //                    var ownerResponse = await _messageRouterManager.SendMessageToPartyByBotAsync(
                    //                        aggregationParty, messageActivity);
                    //                    replyActivityList.Add(ownerResponse);
                    //                }
                    //            }
                    //        }

                    //        if (!conversationClientPartyMissing)
                    //        {
                    //            messageToConversationClient = ConversationText.NotifyClientWaitForRequestHandling;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    var result = _messageRouterManager.RemovePendingRequest(conversationClientParty);
                    //    messageToConversationClient = ConversationText.NotifyClientThatNoAgentIsAvailable;
                    //}
                }
                else if (messageRouterResult.Type == MessageRouterResultType.ConnectionAlreadyRequested)
                {
                    messageToConversationClient = "Your request has already been received, please wait for an agent to respond";
                }
                else if (messageRouterResult.Type == MessageRouterResultType.ConnectionRejected)
                {
                    messageToConversationOwner = string.Format($"Request from user '{0}' rejected", conversationClientName);
                    messageToConversationClient = "Unfortunately your request could not be processed";
                }
                else if (messageRouterResult.Type == MessageRouterResultType.Connected)
                {
                    messageToConversationOwner = string.Format($"You are now connected to user '{0}'", conversationClientName);
                    messageToConversationClient = string.Format($"Your request was accepted and you are now chatting with {0}", conversationOwnerName);
                }
                else if (messageRouterResult.Type == MessageRouterResultType.Disconnected)
                {
                    messageToConversationOwner = string.Format($"You are now disconnected from the conversation with user '{0}'", conversationClientName);
                    messageToConversationClient = string.Format($"Your conversation with {0} has ended", conversationOwnerName);
                }

                if (conversationOwnerParty != null
                    && !string.IsNullOrEmpty(messageToConversationOwner))
                {
                    var ownerResponse = await _messageRouterManager.SendMessageToPartyByBotAsync(
                        conversationOwnerParty, messageToConversationOwner);
                    replyActivityList.Add(ownerResponse);
                }

                if (conversationClientParty != null
                    && !string.IsNullOrEmpty(messageToConversationClient))
                {
                    var clientResponse = await _messageRouterManager.SendMessageToPartyByBotAsync(
                        conversationClientParty, messageToConversationClient);
                    replyActivityList.Add(clientResponse);
                }
                return replyActivityList;
            }
            catch (Exception e)
            {
                Console.WriteLine("Chat bot : Exception in HandleConnectionChangedResultAsync{0}", e.Message);
                return null;
            }
        }
    }
}
