using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Edison.Core.Interfaces;
using Edison.Core.Common.Models;
using Edison.Common.Interfaces;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;
using Edison.ChatService.Config;
using Edison.ChatService.Helpers;
using Edison.ChatService.Models;

namespace Edison.ChatService.Middleware
{
    /// <summary>
    /// Middleware that handles all the command passed to the bot via channeldata, prior to treating the message
    /// </summary>
    public class CommandMiddleware : BaseMiddleware
    {
        private readonly BotOptions _config;
        private readonly IMassTransitServiceBus _serviceBus;
        private readonly IDeviceRestService _deviceRestService;
        private readonly BotRoutingDataManager _routingDataManager;
        private readonly ChatReportDataManager _reportDataManager;
        private readonly ILogger<CommandMiddleware> _logger;

        public CommandMiddleware(IOptions<BotOptions> config, BotRoutingDataManager routingDataManager, IMassTransitServiceBus serviceBus,
             IDeviceRestService deviceRestService, ChatReportDataManager reportDataManager, ILogger<CommandMiddleware> logger) : base(logger)
        {
            _config = config.Value;
            _serviceBus = serviceBus;
            _deviceRestService = deviceRestService;
            _routingDataManager = routingDataManager;
            _reportDataManager = reportDataManager;
            _logger = logger;
        }

        protected override async Task HandleTurnAsync(ITurnContext turnContext, ChatUserContext userContext, 
            MessageRouter messageRouter, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            Activity activity = turnContext.Activity;

            if (activity.TryGetChannelData(out Command command) && command.BaseCommand != Commands.Undefined)
            {
                // Check the activity for commands
                ConversationReference selfConversation = activity.GetConversationReference();
                switch (command.BaseCommand)
                {
                    case Commands.GetTranscript:
                        if (userContext.Role == ChatUserRole.Admin)
                        {
                            var conversations = await _reportDataManager.GetActiveChatReports();
                            var readUsersStatus = await _routingDataManager.GetUsersReadStatusPerUser(userContext.Id);
                            await messageRouter.SendTranscriptAsync(activity.Conversation.Id, conversations, readUsersStatus);
                        }
                        else
                        {
                            var conversation = await _reportDataManager.GetActiveChatReportFromUser(activity.From.Id);
                            await messageRouter.SendTranscriptAsync(activity.Conversation.Id, new List<ChatReportModel>() { conversation });
                        }
                        break;

                    case Commands.ReadUserMessages:
                        //Update a date that shows the last time a message was read by an admin
                        if (userContext.Role == ChatUserRole.Admin)
                        {
                            if (JsonConvert.DeserializeObject<CommandReadUserMessages>(command.Data.ToString()) is CommandReadUserMessages properties)
                            {
                                var result = await _routingDataManager.UpdateUserReadMessageStatus(activity.From.Id, new ChatUserReadStatusModel()
                                {
                                    Date = properties.Date,
                                    UserId = properties.UserId
                                });
                                if (!result)
                                    throw new Exception("Error while executing command ReadUserMessages");
                            }
                        }
                        break;

                    case Commands.EndConversation:
                        //End a conversation by adding an EndDate and delete the user conversation session so broadcast messages aren't sent to them anymore.
                        if (userContext.Role == ChatUserRole.Admin)
                        {
                            if (JsonConvert.DeserializeObject<CommandEndConversation>(command.Data.ToString()) is CommandEndConversation properties)
                            {
                                ChatReportModel userReport = await _reportDataManager.GetActiveChatReportFromUser(properties.UserId);
                                var result = await _reportDataManager.CloseReport(new ChatReportLogCloseModel() { EndDate = DateTime.UtcNow, UserId = properties.UserId });
                                if (result)
                                {
                                    var userConversationReference = await _routingDataManager.GetConversationsFromUser(properties.UserId);
                                    if (userConversationReference != null && userConversationReference.Count() > 0) {
                                        //Warn the user that the conversation is over
                                        await messageRouter.SendMessageAsync(userConversationReference.ToArray()[0],
                                            CommandFactoryHelper.CreateCommandSendMessage(selfConversation.Bot.Id, properties.UserId, selfConversation.User)
                                            , "The conversation was ended. If you have an issue, please start a new conversation.");
                                        
                                        //Call EndOfConversation on the user
                                        await messageRouter.SendEndConversationAsync(messageRouter.CreateEndOfConversationActivity(userConversationReference.ToArray()[0]));
                                        result = await _routingDataManager.DeleteUserConversation(properties.UserId);

                                        //Tell all admins about end of the conversation
                                        var admins = await _routingDataManager.GetAdminConversations();
                                        admins = _routingDataManager.RemoveSelfConversation(admins, selfConversation);
                                        if (admins != null)
                                        {
                                            foreach (var admin in admins)
                                            {
                                                await messageRouter.SendMessageAsync(admin,
                                                   CommandFactoryHelper.CreateCommandEndConversation(selfConversation.Bot.Id, properties.UserId));
                                            }
                                        }

                                        //Send event to event processor... if possible
                                        if (userReport != null)
                                        {
                                            //Sanitize user
                                            string userId = GetDatabaseUserId(userReport.ChannelId, userReport.User.Id);
                                            DeviceModel device = await _deviceRestService.GetMobileDeviceFromUserId(userId);
                                            //Send close message
                                            await PushClosureMessageToEventProcessorSaga(device.DeviceId);
                                        }
                                    }
                                }
                                if (!result)
                                    throw new Exception("Error while executing command EndConversation");
                            }
                        }
                        break;

                    case Commands.SendMessage:
                        if (userContext.Role == ChatUserRole.Admin && !string.IsNullOrWhiteSpace(activity.Text))
                        {
                            if (JsonConvert.DeserializeObject<CommandSendMessageProperties>(command.Data.ToString()) is CommandSendMessageProperties properties)
                            {
                                properties.UserId = properties.UserId;
                                properties.From = userContext; //Forcing admin, in case it isn't given by user
                                turnContext.TurnState.Add(typeof(CommandSendMessageProperties).FullName, properties);
                                //We forward to the next middleware
                                await next(cancellationToken).ConfigureAwait(false);
                            }
                            else
                            {
                                throw new Exception("Error while executing command SendMessage");
                            }
                        }
                        break;

                    default:
                        if (userContext.Role == ChatUserRole.Admin)
                            throw new Exception($"Command not recognized: '{command.BaseCommand}'.");
                        break;
                }
            }
            else if(userContext.Role == ChatUserRole.Consumer && !string.IsNullOrWhiteSpace(activity.Text))
            {
                //Retrieve report type, if provided. Force "Report" if none is provided.
                Guid? reportType = null;
                if (activity.Properties.ContainsKey("reportType") && Guid.TryParse(activity.Properties["reportType"].ToString(), out Guid parsedReportType))
                    reportType = parsedReportType;

                CommandSendMessageProperties properties = new CommandSendMessageProperties()
                {
                    UserId = userContext.Id,
                    From = userContext,
                    ReportType = reportType
                };
                properties.From.Role = ChatUserRole.Consumer; //Forcing consumer
                turnContext.TurnState.Add(typeof(CommandSendMessageProperties).FullName, properties);

                await next(cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<bool> PushClosureMessageToEventProcessorSaga(Guid deviceId)
        {
            try
            {
                if (_serviceBus != null || _serviceBus.BusAccess != null)
                {
                    _logger.LogDebug($"EdisonBot: Pushing for ending conversation.");

                    IEventCloseSagaReceived newMessage = new EventCloseSagaReceivedEvent()
                    {
                        DeviceId = deviceId,
                        EventType = "message"
                    };
                    await _serviceBus.BusAccess.Publish(newMessage);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError($"EdisonBot: {e.Message}");
                return false;
            }
        }
    }
}
