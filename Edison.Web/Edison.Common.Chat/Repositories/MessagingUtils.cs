using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Edison.Common.Chat.Models;
using Edison.Common.Chat.Config;

namespace Edison.Common.Chat.Repositories
{
    public class MessagingUtils
    {
        public MessagingUtils()
        {}

        public struct ConnectorClientAndMessageBundle
        {
            public ConnectorClient connectorClient;
            public IMessageActivity messageActivity;
            
        }
       
        public static Party CreateSenderParty(Activity activity, bool withTimestamps = true)
        {
            if (withTimestamps)
            {
                var party = new Party(activity.ServiceUrl, activity.ChannelId, activity.From, activity.Conversation)
                {
                    MessageRequestString = activity.Text
                };
                return party;
            }

            return new Party(activity.ServiceUrl, activity.ChannelId, activity.From, activity.Conversation);
        }

        public static Party CreateRecipientParty(IActivity activity, bool withTimestamps = true)
        {
            if (withTimestamps)
            {
                return new Party(activity.ServiceUrl, activity.ChannelId, activity.Recipient, activity.Conversation);
            }

            return new Party(activity.ServiceUrl, activity.ChannelId, activity.Recipient, activity.Conversation);
        }

        public static async Task ReplyToActivityAsync(Activity activity, string message)
        {
            if (activity != null && !string.IsNullOrEmpty(message))
            {
                Activity replyActivity = activity.CreateReply(message);
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                await connector.Conversations.ReplyToActivityAsync(replyActivity);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Either the activity is null or the message is empty - Activity: {activity}; message: {message}");
            }
        }

        //public static ConnectorClientAndMessageBundle CreateConnectorClientAndMessageActivity(
        //    string serviceUrl, IMessageActivity newMessageActivity,IOptions<BotOptions> botOptions)
        //{
        //    var appCredentials = new MicrosoftAppCredentials();
        //    appCredentials.MicrosoftAppId = botOptions.Value.MicrosoftAppId;
        //    appCredentials.MicrosoftAppPassword = botOptions.Value.MicrosoftAppPassword;
        //    MicrosoftAppCredentials.TrustServiceUrl(serviceUrl, DateTime.Now.AddDays(7));
        //    ConnectorClient newConnectorClient = new ConnectorClient(new Uri(serviceUrl), appCredentials);

        //    ConnectorClientAndMessageBundle bundle = new ConnectorClientAndMessageBundle()
        //    {
        //        connectorClient = newConnectorClient,
        //        messageActivity = newMessageActivity
        //    };

        //    return bundle;
        //}

        
        //public static ConnectorClientAndMessageBundle CreateConnectorClientAndMessageActivity(
        //    Party partyToMessage, string messageText, ChannelAccount senderAccount, IOptions<BotOptions> botOptions)
        //{
        //    IMessageActivity newMessageActivity = Activity.CreateMessageActivity();
        //    newMessageActivity.Conversation = partyToMessage.ConversationAccount;
        //    newMessageActivity.Text = messageText;

        //    if (senderAccount != null)
        //    {
        //        newMessageActivity.From = senderAccount;
        //    }

        //    if (partyToMessage.ChannelAccount != null)
        //    {
        //        newMessageActivity.Recipient = partyToMessage.ChannelAccount;
        //    }

        //    return CreateConnectorClientAndMessageActivity(partyToMessage.ServiceUrl, newMessageActivity, botOptions);
        //}

        public static IMessageActivity CreateMessageActivity(
            Party partyToMessage, string messageText, ChannelAccount senderAccount, IOptions<BotOptions> botOptions)
        {
            IMessageActivity newMessageActivity = Activity.CreateMessageActivity();
            newMessageActivity.Conversation = partyToMessage.ConversationAccount;
            newMessageActivity.Text = messageText;

            if (senderAccount != null)
            {
                newMessageActivity.From = senderAccount;
            }

            if (partyToMessage.ChannelAccount != null)
            {
                newMessageActivity.Recipient = partyToMessage.ChannelAccount;
            }

            return newMessageActivity;
        }

        public static string StripMentionsFromMessage(IMessageActivity messageActivity)
        {
            string strippedMessage = messageActivity.Text;

            if (!string.IsNullOrEmpty(strippedMessage))
            {
                Mention[] mentions = messageActivity.GetMentions();

                foreach (Mention mention in mentions)
                {
                    string mentionText = mention.Text;

                    if (!string.IsNullOrEmpty(mentionText))
                    {
                        while (strippedMessage.Contains(mentionText))
                        {
                            strippedMessage = strippedMessage.Remove(
                                strippedMessage.IndexOf(mentionText), mentionText.Length);
                        }

                    }
                }

                strippedMessage = strippedMessage.Trim();
            }

            return strippedMessage;
        }
    }
}