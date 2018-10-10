using Edison.Common.Chat.CommandHandling;
using Edison.Common.Chat.Config;
using Edison.Common.Chat.MessageRouting;
using Edison.Common.Chat.Models.Interface;
using Edison.Common.Chat.Repositories;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Api
{
    public static class StartupMessageRouting
    {
        public static MessageRouterManager MessageRouterManager { get; set; }

        public static MessageRouterResultHandler MessageRouterResultHandler { get; set; }

        public static BackChannelMessageHandler BackChannelMessageHandler { get; set; }

        public static void InitializeMessageRouting(IOptions<BotOptions> botOptions,
            IChatRoutingDataManagerRepository routingDataManager,
            IConversationChatBot conversationChatBot)
        {
            MessageRouterManager = new MessageRouterManager(routingDataManager, botOptions, conversationChatBot);
            MessageRouterResultHandler = new MessageRouterResultHandler(MessageRouterManager);
            BackChannelMessageHandler = new BackChannelMessageHandler(MessageRouterManager._routingDataManager);
        }
    }
}
