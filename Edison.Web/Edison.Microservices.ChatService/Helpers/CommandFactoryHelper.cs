using System;
using AutoMapper;
using Microsoft.Bot.Schema;
using Edison.Core.Common.Models;
using Edison.ChatService.Models;

namespace Edison.ChatService.Helpers
{
    public static class CommandFactoryHelper
    {
        public static Command CreateCommandError(string botId)
        {
            return new Command()
            {
                BaseCommand = Commands.Error,
                BotName = botId
            };
        }

        public static Command CreateCommandReadUserMessages(string botId, string userId, DateTime date)
        {
            return CreateCommandReadUserMessages(botId, new CommandReadUserMessages()
            {
                UserId = userId,
                Date = date
            });
        }

        public static Command CreateCommandReadUserMessages(string botId, CommandReadUserMessages sendReadUserMessages)
        {
            return new Command()
            {
                BaseCommand = Commands.ReadUserMessages,
                BotName = botId,
                Data = sendReadUserMessages
            };
        }

        public static Command CreateCommandEndConversation(string botId, string userId)
        {
            return CreateCommandEndConversation(botId, new CommandEndConversation()
            {
                UserId = userId
            });
        }

        public static Command CreateCommandEndConversation(string botId, CommandEndConversation sendEndConversation)
        {
            return new Command()
            {
                BaseCommand = Commands.EndConversation,
                BotName = botId,
                Data = sendEndConversation
            };
        }

        public static Command CreateCommandSendMessage(string botId, CommandSendMessageProperties sendMessageProperties)
        {
            return new Command()
            {
                BaseCommand = Commands.SendMessage,
                BotName = botId,
                Data = sendMessageProperties
            };
        }

        public static Command CreateCommandSendMessage(string botId, string userId, ChatUserModel from, Guid? reportType = null)
        {
            return CreateCommandSendMessage(botId, new CommandSendMessageProperties()
            {
                UserId = userId,
                From = from,
                ReportType = reportType
            });
        }

        public static Command CreateCommandSendMessage(string botId, string userId, ChannelAccount from, Guid? reportType = null)
        {
            return CreateCommandSendMessage(botId, userId, Mapper.Map<ChatUserModel>(from), reportType);
        }
    }
}
