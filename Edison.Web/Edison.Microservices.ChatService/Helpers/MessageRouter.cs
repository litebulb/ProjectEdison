using Edison.ChatService.Models;
using Edison.Core.Common.Models;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.ChatService.Helpers
{
    public class MessageRouter
    {
        private readonly ILogger _logger;
        private readonly ConnectorClient _botClient;
        private readonly Activity _currentActivity;

        public MessageRouter(ConnectorClient botClient, Activity currentActivity, ILogger logger)
        {
            _botClient = botClient;
            _currentActivity = currentActivity;
            _logger = logger;
        }

        #region Send Message
        public async Task<ResourceResponse> SendMessageAsync(IMessageActivity activity)
        {
            if (activity == null)
            {
                _logger.LogError("The activity is null");
                return null;
            }

            ResourceResponse resourceResponse = null;

            try
            {
                resourceResponse = await _botClient.Conversations.SendToConversationAsync((Activity)activity);
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogError($"Failed to send message: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to send message: {e.Message}");
            }

            return resourceResponse;
        }

        public async Task<ResourceResponse> SendMessageAsync(ConversationReference recipient, CommandSendMessageProperties sendMessageProperties, string message = "")
        {
            IMessageActivity messageActivity = CreateMessageActivity(recipient, sendMessageProperties, message);
            return await SendMessageAsync(messageActivity);
        }

        public async Task<ResourceResponse> SendMessageAsync(ConversationReference recipient, Command command, string message = "")
        {
            IMessageActivity messageActivity = CreateMessageActivity(recipient, command, message);
            return await SendMessageAsync(messageActivity);
        }

        public async Task<ResourceResponse> SendErrorMessageAsync(IMessageActivity activity)
        {
            if (activity == null)
            {
                _logger.LogError("The activity is null");
                return null;
            }

            ResourceResponse resourceResponse = null;

            try
            {
                resourceResponse = await _botClient.Conversations.SendToConversationAsync((Activity)activity);
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogError($"Failed to send message: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to send message: {e.Message}");
            }

            return resourceResponse;
        }

        public async Task<ResourceResponse> SendErrorMessageAsync(ConversationReference recipient, string message)
        {
            IMessageActivity errorActivity = CreateErrorActivity(recipient, message);
            return await SendErrorMessageAsync(errorActivity);
        }

        public async Task<ResourceResponse> SendEndConversationAsync(IEndOfConversationActivity activity)
        {
            if (activity == null)
            {
                _logger.LogError("The activity is null");
                return null;
            }

            ResourceResponse resourceResponse = null;

            try
            {
                resourceResponse = await _botClient.Conversations.SendToConversationAsync((Activity)activity);
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogError($"Failed to send message: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to send message: {e.Message}");
            }

            return resourceResponse;
        }
        #endregion

        #region Transcript
        public async Task<ResourceResponse> SendTranscriptAsync(
            string conversationId, IEnumerable<ReportModel> conversations,
            IEnumerable<ChatUserReadStatusModel> usersReadStatus = null)
        {
            var transcriptActivities = GetActivityFromConversationLogs(conversations);
            if (usersReadStatus != null)
            {
                var usersReadMessages = GetActivityFromUsersReadMessages(usersReadStatus);
                transcriptActivities = transcriptActivities.Concat(usersReadMessages).ToList();
            }
            
            return await SendTranscriptAsync(conversationId, transcriptActivities);
        }

        public async Task<ResourceResponse> SendTranscriptAsync(
            string conversationId, IList<Activity> activities)
        {
            ResourceResponse resourceResponse = null;

            if (activities.Count == 0)
                return resourceResponse;

            try
            {
                Transcript transcript = new Transcript(activities);
                resourceResponse = await _botClient.Conversations.SendConversationHistoryAsync(conversationId, transcript);
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogError($"Failed to send message: {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to send message: {e.Message}");
                return null;
            }

            return resourceResponse;
        }

        public IList<Activity> GetActivityFromConversationLogs(IEnumerable<ReportModel> reports)
        {
            if (reports == null)
                return null;

            ConversationReference selfConversation = _currentActivity.GetConversationReference();
            IList<Activity> replayTranscriptActivities = new List<Activity>();
            List<string> listBroadcastIds = new List<string>();
            ConversationAccount conversationAccount = new ConversationAccount(null, null, _currentActivity.Conversation.Id, null, null);
            foreach (ReportModel report in reports)
            {
                foreach (ReportLogModel reportLog in report.ReportLogs)
                {
                    Activity activityLog = new Activity();
                    activityLog.ApplyConversationReference(selfConversation);

                    //Dealing with broadcast. If broadcast, we ensure that the message is only send once
                    if (reportLog.IsBroadcast)
                    {
                        if (!listBroadcastIds.Contains(reportLog.Id))
                        {
                            listBroadcastIds.Add(reportLog.Id);
                            activityLog.ChannelData = CommandFactoryHelper.CreateCommandSendMessage(
                        selfConversation.Bot.Id, "*", reportLog.From, report.ReportType);
                        } else
                            continue;
                    }
                    else
                    {
                        activityLog.ChannelData = CommandFactoryHelper.CreateCommandSendMessage(
                            selfConversation.Bot.Id, report.User.Id, reportLog.From, report.ReportType);
                    }

                    //Rest of the message
                    activityLog.Id = reportLog.Id;
                    activityLog.ReplyToId = null;
                    activityLog.Text = reportLog.Message;
                    activityLog.Type = ActivityTypes.Message;
                    activityLog.Conversation = conversationAccount;
                    activityLog.Timestamp = reportLog.Date;
                    activityLog.LocalTimestamp = reportLog.Date;
                    replayTranscriptActivities.Add(activityLog);
                }
            }

            return replayTranscriptActivities.OrderBy(p => p.Timestamp).ToList();
        }

        public IList<Activity> GetActivityFromUsersReadMessages(IEnumerable<ChatUserReadStatusModel> usersReadMessages)
        {
            if (usersReadMessages == null)
                return null;

            ConversationReference selfConversation = _currentActivity.GetConversationReference();
            ConversationAccount conversationAccount = new ConversationAccount(null, null, _currentActivity.Conversation.Id, null, null);
            IList<Activity> usersReadMessagesActivities = new List<Activity>();
            foreach (ChatUserReadStatusModel userReadMessages in usersReadMessages)
            {
                Activity activityLog = new Activity();
                activityLog.ApplyConversationReference(selfConversation);
                activityLog.ReplyToId = null;
                activityLog.Text = string.Empty;
                activityLog.Type = ActivityTypes.Message;
                activityLog.Conversation = conversationAccount;
                activityLog.Timestamp = DateTime.UtcNow;
                activityLog.LocalTimestamp = DateTime.UtcNow;
                activityLog.ChannelData = CommandFactoryHelper.CreateCommandReadUserMessages(
                    selfConversation.Bot.Id, new CommandReadUserMessages() { UserId = userReadMessages.UserId, Date = userReadMessages.Date });
                usersReadMessagesActivities.Add(activityLog);
            }

            return usersReadMessagesActivities;
        }
        #endregion

        #region Messages Helpers
        public IMessageActivity CreateMessageActivity(ConversationReference conversation, CommandSendMessageProperties sendMessageProperties, string message)
        {
            IMessageActivity messageActivity = Activity.CreateMessageActivity();

            if (conversation != null)
            {
                if (conversation.Conversation != null)
                {
                    messageActivity.Conversation = conversation.Conversation;
                }

                if (conversation.User != null)
                {
                    messageActivity.Recipient = conversation.User;
                }

                if (conversation.Bot != null)
                {
                    messageActivity.From = conversation.Bot;
                }
            }

            messageActivity.ChannelData = CommandFactoryHelper.CreateCommandSendMessage(conversation.Bot.Id, sendMessageProperties);
            messageActivity.Text = message;
            return messageActivity;
        }

        public IMessageActivity CreateMessageActivity(ConversationReference conversation, Command command, string message)
        {
            IMessageActivity messageActivity = Activity.CreateMessageActivity();

            if (conversation != null)
            {
                if (conversation.Conversation != null)
                {
                    messageActivity.Conversation = conversation.Conversation;
                }

                if (conversation.User != null)
                {
                    messageActivity.Recipient = conversation.User;
                }

                if (conversation.Bot != null)
                {
                    messageActivity.From = conversation.Bot;
                }
            }

            messageActivity.ChannelData = command;
            messageActivity.Text = message;
            return messageActivity;
        }

        public IMessageActivity CreateErrorActivity(ConversationReference conversation, string message)
        {
            IMessageActivity errorActivity = Activity.CreateMessageActivity();

            if (conversation != null)
            {
                if (conversation.Conversation != null)
                {
                    errorActivity.Conversation = conversation.Conversation;
                }

                if (conversation.User != null)
                {
                    errorActivity.Recipient = conversation.User;
                }

                if (conversation.Bot != null)
                {
                    errorActivity.From = conversation.Bot;
                }
            }

            errorActivity.ChannelData = CommandFactoryHelper.CreateCommandError(conversation.Bot.Id);
            errorActivity.Text = message;
            return errorActivity;
        }

        public IEndOfConversationActivity CreateEndOfConversationActivity(ConversationReference conversation)
        {
            IEndOfConversationActivity endActivity = Activity.CreateEndOfConversationActivity();

            if (conversation != null)
            {
                if (conversation.Conversation != null)
                {
                    endActivity.Conversation = conversation.Conversation;
                }

                if (conversation.User != null)
                {
                    endActivity.Recipient = conversation.User;
                }

                if (conversation.Bot != null)
                {
                    endActivity.From = conversation.Bot;
                }
            }

            endActivity.Text = string.Empty;
            return endActivity;
        }
        #endregion
    }
}
