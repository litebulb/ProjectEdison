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
using AutoMapper;
using Newtonsoft.Json;

namespace Edison.ChatService.Middleware
{
    public class LogMessageMiddleware : BaseMiddleware
    {
        private readonly BotOptions _config;
        private readonly IMassTransitServiceBus _serviceBus;
        private readonly BotRoutingDataManager _routingDataManager;
        private readonly ChatReportDataManager _reportDataManager;
        private readonly ILogger<CommandMiddleware> _logger;

        public LogMessageMiddleware(IOptions<BotOptions> config, BotRoutingDataManager routingDataManager, 
            ChatReportDataManager reportDataManager, IMassTransitServiceBus serviceBus,
        ILogger<CommandMiddleware> logger) : base(logger)
        {
            _config = config.Value;
            _serviceBus = serviceBus;
            _routingDataManager = routingDataManager;
            _reportDataManager = reportDataManager;
            _logger = logger;
        }

        protected override async Task HandleTurnAsync(ITurnContext turnContext, ChatUserContext userContext, 
            MessageRouter messageRouter, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            Activity activity = turnContext.Activity;

            if(turnContext.TurnState.TryGetValue(typeof(CommandSendMessageProperties).FullName, out object result))
            {
                CommandSendMessageProperties properties = (CommandSendMessageProperties)result;

                //Log broadcast
                if (properties.From.Role == ChatUserRole.Admin && properties.UserId == "*")
                {
                    var allConsumerConservations = await _routingDataManager.GetConsumerConversations();
                    foreach(var consumerConversation in allConsumerConservations)
                    {
                        await _reportDataManager.CreateOrUpdateChatReport(new ChatReportLogCreationModel()
                        {
                            User = properties.From.Role == ChatUserRole.Admin ? new ChatUserModel() { Id = consumerConversation.User.Id } : properties.From,
                            Message = new ChatReportLogModel()
                            {
                                From = properties.From,
                                Date = activity.Timestamp.Value.DateTime,
                                Message = activity.Text,
                                ReportType = properties.ReportType,
                                Id = activity.Id,
                                IsBroadcast = true
                            }
                        });
                    }
                }
                else
                {
                    //Log one message
                    ChatReportModel chatReportModel = await _reportDataManager.CreateOrUpdateChatReport(new ChatReportLogCreationModel()
                    {
                        User = properties.From.Role == ChatUserRole.Admin ? new ChatUserModel() { Id = properties.UserId } : properties.From,
                        ChannelId = activity.ChannelId,
                        Message = new ChatReportLogModel()
                        {
                            From = properties.From,
                            Date = activity.Timestamp.Value.DateTime,
                            Message = activity.Text,
                            ReportType = properties.ReportType,
                            Id = activity.Id
                        }
                    });
                    turnContext.TurnState.Add(typeof(ChatReportModel).FullName, chatReportModel);
                }

                await next(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
