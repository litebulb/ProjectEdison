using Edison.ChatService.Config;
using Edison.ChatService.Helpers;
using Edison.Common.Interfaces;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Microsoft.Bot.Schema;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.ChatService.Models;
using System.Collections.Generic;
using Edison.Common.Messages.Interfaces;
using Newtonsoft.Json;
using Edison.Common.Messages;
using Edison.Core.Interfaces;

namespace Edison.ChatService.Middleware
{
    public class HandoffMiddleware : BaseMiddleware
    {
        private readonly BotOptions _config;
        private readonly IMassTransitServiceBus _serviceBus;
        private readonly IDeviceRestService _deviceRestService;
        private readonly BotRoutingDataManager _routingDataManager;
        private readonly ChatReportDataManager _reportDataManager;
        private readonly ILogger<CommandMiddleware> _logger;

        public HandoffMiddleware(IOptions<BotOptions> config, IDeviceRestService deviceRestService,
            BotRoutingDataManager routingDataManager, ChatReportDataManager reportDataManager, IMassTransitServiceBus serviceBus,
        ILogger<CommandMiddleware> logger) : base(logger)
        {
            _config = config.Value;
            _deviceRestService = deviceRestService;
            _reportDataManager = reportDataManager;
            _serviceBus = serviceBus;
            _routingDataManager = routingDataManager;
            _logger = logger;
        }

        protected override async Task HandleTurnAsync(ITurnContext turnContext, ChatUserContext userContext, 
            MessageRouter messageRouter, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            Activity activity = turnContext.Activity;

            if (turnContext.TurnState.TryGetValue(typeof(CommandSendMessageProperties).FullName, out object result))
            {
                CommandSendMessageProperties properties = (CommandSendMessageProperties)result;
                ConversationReference selfConversation = activity.GetConversationReference();

                //Broadcast to many users
                IEnumerable<ConversationReference> conversations = await GetHandoffConversation(turnContext, activity, properties, selfConversation);

                //Send messages
                if (conversations != null)
                    foreach (var conversation in conversations)
                        await messageRouter.SendMessageAsync(conversation, properties, activity.Text);

                await next(cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<IEnumerable<ConversationReference>> GetHandoffConversation(ITurnContext turnContext, 
            Activity activity, CommandSendMessageProperties properties, ConversationReference selfConversation)
        {
            //Broadcast to many users
            IEnumerable<ConversationReference> conversations = null;
            if (properties.From.Role == ChatUserRole.Consumer)
            {
                conversations = await _routingDataManager.GetAdminConversations();
                //Send event to event processor... if possible
                if (turnContext.TurnState.TryGetValue(typeof(ChatReportModel).FullName, out object result))
                    await PushMessageToEventProcessorSaga(activity.Text, activity.ChannelId, properties, (ChatReportModel)result);
            }
            //To one user only - Admin only
            else if (properties.From.Role == ChatUserRole.Admin)
            {
                if (properties.UserId == "*")
                {
                    conversations = await _routingDataManager.GetConversations();
                }
                else
                {
                    conversations = await _routingDataManager.GetConversationsFromUser(properties.UserId);
                    IEnumerable<ConversationReference> conversations2 = await _routingDataManager.GetAdminConversations();
                    conversations = conversations2.Union(conversations);
                }
            }
            else
            {
                throw new Exception("Message handoff not handled");
            }
            //Do not send to self
            conversations = _routingDataManager.RemoveSelfConversation(conversations, selfConversation);

            return conversations;
        }

        private async Task<bool> PushMessageToEventProcessorSaga(string message, string channelId, CommandSendMessageProperties consumerMessageProperties, ChatReportModel chatReport)
        {
            try
            {
                if (_serviceBus != null || _serviceBus.BusAccess != null)
                {
                    _logger.LogDebug($"EdisonBot: Pushing message from user '{consumerMessageProperties.From.Id}'.");

                    //Get deviceId
                    string userId = GetDatabaseUserId(channelId, consumerMessageProperties.UserId);
                    DeviceModel device = await _deviceRestService.GetMobileDeviceFromUserId(userId);

                    //Get last reportType
                    Guid? reportType = consumerMessageProperties.ReportType;
                    if(consumerMessageProperties.ReportType == null || consumerMessageProperties.ReportType == Guid.Empty)
                        reportType = await GetLastReportTypeFromUser(consumerMessageProperties.UserId);

                    if (device != null)
                    {
                        IEventSagaReceived newMessage = new EventSagaReceivedEvent()
                        {
                            DeviceId = device.DeviceId,
                            EventType = "message",
                            Date = DateTime.UtcNow,
                            CheckBoundary = consumerMessageProperties.ReportType == _config.EmergencyActionPlanId,
                            Data = JsonConvert.SerializeObject(new MessageEventMetadata()
                            {
                                UserId = consumerMessageProperties.UserId,
                                Username = consumerMessageProperties.From.Name,
                                ReportType = reportType,
                                Message = message,
                                ChatReportId = chatReport.ReportId
                            })
                        };
                        await _serviceBus.BusAccess.Publish(newMessage);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError($"EdisonBot: {e.Message}");
                return false;
            }
        }

        private async Task<Guid?> GetLastReportTypeFromUser(string userId)
        {
            Guid? reportType = null;

            ChatReportModel activeReport = await _reportDataManager.GetActiveChatReportFromUser(userId);
            if (activeReport != null)
            {
                for (int i = activeReport.ReportLogs.Count - 1; 0 <= i; i--)
                {
                    Guid? reportLog = activeReport.ReportLogs[i].ReportType;
                    if (reportLog != null && reportLog != Guid.Empty)
                    {
                        reportType = reportLog;
                        break;
                    }
                }
            }
            return reportType;
        }
    }
}
