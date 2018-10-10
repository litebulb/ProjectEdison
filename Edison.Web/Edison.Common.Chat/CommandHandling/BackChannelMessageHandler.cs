using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using Edison.Core.Common;
using Edison.Common.Chat.Models.Interface;
using Edison.Common.Chat.Models;
using Edison.Common.Chat.Repositories;

namespace Edison.Common.Chat.CommandHandling
{
    public class BackChannelMessageHandler
    {
        public const string DefaultBackChannelId = "backchannel";
        public const string DefaultPartyKey = "conversationId";

        public string BackChannelId
        {
            get;
            protected set;
        }

        public string PartyKey
        {
            get;
            protected set;
        }

        private IChatRoutingDataManagerRepository _routingDataManager;

        public BackChannelMessageHandler(IChatRoutingDataManagerRepository routingDataManager, string backChannelId = null, string partyKey = null)
        {
            _routingDataManager = routingDataManager
                ?? throw new ArgumentNullException("Routing data manager instance must be given");

            BackChannelId = string.IsNullOrEmpty(backChannelId) ? DefaultBackChannelId : backChannelId;
            PartyKey = string.IsNullOrEmpty(partyKey) ? DefaultPartyKey : partyKey;
        }

        public virtual MessageRouterResult HandleBackChannelMessage(Activity activity)
        {
            MessageRouterResult messageRouterResult = new MessageRouterResult();

            if (activity == null || string.IsNullOrEmpty(activity.Text))
            {
                messageRouterResult.Type = MessageRouterResultType.Error;
                messageRouterResult.ErrorMessage = $"The given activity ({nameof(activity)}) is either null or the message is missing";
            }
            else if (activity.Text.Equals(BackChannelId))
            {
                if (activity.ChannelData == null)
                {
                    messageRouterResult.Type = MessageRouterResultType.Error;
                    messageRouterResult.ErrorMessage = "No channel data";
                }
                else
                {
                    // Handle accepted request and start 1:1 conversation
                    Party conversationClientParty = null;

                    try
                    {
                        conversationClientParty = ParsePartyFromChannelData(activity.ChannelData);
                    }
                    catch (Exception e)
                    {
                        messageRouterResult.Type = MessageRouterResultType.Error;
                        messageRouterResult.ErrorMessage =
                            $"Failed to parse the party information from the back channel message: {e.Message}";
                    }

                    if (conversationClientParty != null)
                    {
                        Party conversationOwnerParty = MessagingUtils.CreateSenderParty(activity);

                        messageRouterResult = _routingDataManager.ConnectAndClearPendingRequestAsync(
                            conversationOwnerParty, conversationClientParty);

                        messageRouterResult.Activity = activity;
                    }
                }
            }
            else
            {
                // No back channel message detected
                messageRouterResult.Type = MessageRouterResultType.NoActionTaken;
            }

            return messageRouterResult;
        }

        protected Party ParsePartyFromChannelData(object channelData)
        {
            string partyAsJsonString = ((JObject)channelData)[BackChannelId][PartyKey].ToString();

            if (string.IsNullOrEmpty(partyAsJsonString))
            {
                throw new NullReferenceException("Failed to find the party information from the channel data");
            }

            partyAsJsonString = partyAsJsonString.Replace(StringAndCharConstants.EndOfLineInJsonResponse, string.Empty);
            partyAsJsonString = partyAsJsonString.Replace(StringAndCharConstants.BackslashInJsonResponse, StringAndCharConstants.QuotationMark);
            return Party.FromJsonString(partyAsJsonString);
        }
    }
}