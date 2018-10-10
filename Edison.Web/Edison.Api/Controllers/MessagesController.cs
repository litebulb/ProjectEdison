using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edison.Common.Chat.CommandHandling;
using Edison.Common.Chat.Config;
using Edison.Common.Chat.MessageRouting;
using Edison.Common.Chat.Models;
using Edison.Common.Chat.Models.Interface;
using Edison.Common.Chat.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Edison.Api.Controllers
{
    //[Authorize(AuthenticationSchemes = "Backend,B2CWeb")]
    [Route("api/messages")]
    [ApiController]
    public class MessagesController : Controller
    {
        private static IConfiguration _configuration;
        private static IConversationChatBot _conversationChatBot;
       private static IOptions<BotOptions> _botOptions;
        private static MicrosoftAppCredentials _credentials;
        private static MessageRouterManager _messageRouterManager;
        private static MessageRouterResultHandler _messageRouterResultHandler;

        public MessagesController(IConfiguration configuration, IConversationChatBot conversationChatBot,
            IOptions<BotOptions> botOptions)
        {
            _configuration = configuration;
            _conversationChatBot = conversationChatBot;
            _botOptions = botOptions;
            _credentials = new MicrosoftAppCredentials()
            {
                MicrosoftAppId = botOptions.Value.MicrosoftAppId,
                MicrosoftAppPassword = botOptions.Value.MicrosoftAppPassword
            };
            _messageRouterManager = StartupMessageRouting.MessageRouterManager;
            _messageRouterResultHandler = StartupMessageRouting.MessageRouterResultHandler;
        }

        [Authorize(Roles = "Bot")]
        [HttpPost]
        public async Task<string> Post([FromBody] Activity activity)
        {
            string responseText = "";

            if (activity.Type == ActivityTypes.Message)
            {
                Console.WriteLine("Chat bot : Message received successfully {0}", activity.Text);

                _messageRouterManager.MakeSurePartiesAreTracked(activity);

                // First check for commands (both from back channel and the ones directly typed)
                MessageRouterResult messageRouterResult =
                    StartupMessageRouting.BackChannelMessageHandler.HandleBackChannelMessage(activity);

                responseText = await HandleActivity(activity, messageRouterResult);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return responseText;
        }


        private async Task<string> HandleActivity(Activity activity, MessageRouterResult messageRouterResult)
        {
            var connector = _conversationChatBot.Create(new Uri(activity.ServiceUrl), _credentials);
            bool rejectConnectionRequestIfNoAggregationChannel =
                        _botOptions.Value.RejectConnectionRequestIfNoAggregationChannel;

            if (messageRouterResult.Type != MessageRouterResultType.Connected)
            {
                // for handling agent bot conversations
                messageRouterResult = await _messageRouterManager.HandleActivityAsync(
                    activity, false, rejectConnectionRequestIfNoAggregationChannel);


                if (messageRouterResult.Type == MessageRouterResultType.NoActionTaken)
                {
                    if (!string.IsNullOrEmpty(activity.Text)
                        && activity.Text.ToLower().Contains(Commands.CommandRequestConnection))
                    {
                        messageRouterResult = _messageRouterManager.RequestConnection(
                            activity, rejectConnectionRequestIfNoAggregationChannel);
                    }
                    else
                    {
                        // check admin
                        if (activity.From.Id.Contains("edisonadmin.com"))
                        {
                            // admin messages
                        }
                        else
                        {
                            messageRouterResult = _messageRouterManager.RequestConnection(
                        activity, rejectConnectionRequestIfNoAggregationChannel);

                            var replyMessageActivity = await _messageRouterResultHandler.HandleConnectionChangedResultAsync(messageRouterResult);
                            if (replyMessageActivity.Count > 0)
                                return replyMessageActivity[0];
                        }
                    }
                }
            }
            return null;
        }

        private async void SendAutoWelcomeResponse(Activity activity)
        {
            var connector = _conversationChatBot.Create(new Uri(activity.ServiceUrl), _credentials);

            var autoReply = "Hi, please state the issue you are currently facing.";

            /*HeroCard card = new HeroCard
            {
                Title = "Report Activity",
                Subtitle = autoReply
            };*/

            var replyActivity = activity.CreateReply();
            replyActivity.Text = autoReply;
            //replyActivity.Attachments.Add(card.ToAttachment());
            await SendReplyActivity(connector, replyActivity);
        }

        private async Task<string> SendReplyActivity(IConnectorClient connector, Activity replyActivity)
        {
            var reply = await _conversationChatBot.SendReply(connector, replyActivity);
            Console.WriteLine("Chat bot : Message replied successfully {0}", replyActivity.Text);
            return reply;
        }

        private void HandleSystemMessage(Activity activity)
        {
            MessageRouterManager messageRouterManager = StartupMessageRouting.MessageRouterManager;

            if (activity.Type == ActivityTypes.DeleteUserData)
            {
                Party senderParty = MessagingUtils.CreateSenderParty(activity);
                IList<MessageRouterResult> messageRouterResults = messageRouterManager.RemoveParty(senderParty);

                foreach (MessageRouterResult messageRouterResult in messageRouterResults)
                {
                    if (messageRouterResult.Type == MessageRouterResultType.OK)
                    {
                        Console.WriteLine($"Data of user '{0}' deleted", senderParty.ChannelAccount?.Name);
                    }
                }
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                IConversationUpdateActivity iConversationUpdated = activity;
                if (iConversationUpdated != null)
                {
                    ConnectorClient connector = new ConnectorClient(new System.Uri(activity.ServiceUrl));

                    foreach (var member in iConversationUpdated.MembersAdded ?? System.Array.Empty<ChannelAccount>())
                    {
                        // if the bot is added, then
                        if (member.Id == iConversationUpdated.Recipient.Id)
                        {
                            SendAutoWelcomeResponse(activity);
                        }
                    }
                }

                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                if (activity.MembersRemoved != null && activity.MembersRemoved.Count > 0)
                {
                    foreach (ChannelAccount channelAccount in activity.MembersRemoved)
                    {
                        Party partyToRemove = new Party(activity.ServiceUrl, activity.ChannelId, channelAccount, activity.Conversation);
                        IList<MessageRouterResult> messageRouterResults = messageRouterManager.RemoveParty(partyToRemove);

                        foreach (MessageRouterResult messageRouterResult in messageRouterResults)
                        {
                            if (messageRouterResult.Type == MessageRouterResultType.OK)
                            {
                                Console.WriteLine($"Party '{0}' removed", partyToRemove.ChannelAccount?.Name);
                            }
                        }
                    }
                }
            }
            else if (activity.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (activity.Type == ActivityTypes.Typing)
            {
                // Handle knowing that the user is typing
            }
        }
    }
}
