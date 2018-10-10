using Edison.Common.Chat.Models;
using Edison.Common.Chat.Models.Interface;
using Edison.Common.Chat.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Common.Chat.MessageRouting
{
    public class MessageRoutingUtils
    {
        private const string ChannelIdEmulator = "emulator";
        private const string ChannelIdFacebook = "facebook";
        private const string ChannelIdSkype = "skype";

        private readonly IList<string> NoDirectConversationsWithChannels = new List<string>()
        {
            ChannelIdEmulator,
            ChannelIdFacebook,
            ChannelIdSkype
        };

        public static async Task BroadcastMessageToAggregationChannelsAsync(
            MessageRouterManager messageRouterManager, string messageText)
        {
            foreach (Party aggregationChannel in
                messageRouterManager._routingDataManager.GetAgentParties())
            {
                await messageRouterManager.SendMessageToPartyByBotAsync(aggregationChannel, messageText);
            }
        }

        public async Task<string> AcceptOrRejectRequestAsync(
            MessageRouterManager messageRouterManager, MessageRouterResultHandler messageRouterResultHandler,
            Party senderParty, bool doAccept, string channelAccountIdOfPartyToAcceptOrReject)
        {
            string errorMessage = null;

            IChatRoutingDataManagerRepository routingDataManager = messageRouterManager._routingDataManager;
            Party partyToAcceptOrReject = null;

            if (routingDataManager.GetPendingRequests().Count > 0)
            {
                try
                {
                    partyToAcceptOrReject = routingDataManager.GetPendingRequests().Single(
                          party => (party.ChannelAccount != null
                              && !string.IsNullOrEmpty(party.ChannelAccount.Id)
                              && party.ChannelAccount.Id.Equals(channelAccountIdOfPartyToAcceptOrReject)));
                }
                catch (InvalidOperationException e)
                {
                    errorMessage = string.Format(
                        $"Failed to find a pending request for user '0': {1}",
                        channelAccountIdOfPartyToAcceptOrReject,
                        e.Message);
                }
            }

            if (partyToAcceptOrReject != null)
            {
                Party connectedSenderParty =
                routingDataManager.FindConnectedPartyByChannel(
                    senderParty.ChannelId, senderParty.ChannelAccount);

                bool senderIsConnected =
                    (connectedSenderParty != null
                    && routingDataManager.IsConnected(connectedSenderParty, ConnectionProfileType.Agent));

                MessageRouterResult messageRouterResult = null;

                if (doAccept)
                {
                    if (senderIsConnected)
                    {
                        // The sender (accepter/rejecter) is ALREADY connected with another party
                        Party otherParty = routingDataManager.GetConnectedCounterpart(connectedSenderParty);

                        if (otherParty != null)
                        {
                            errorMessage = string.Format(
                                $"You are already connected with user '{0}'", otherParty.ChannelAccount?.Name);
                        }
                        else
                        {
                            errorMessage = "An error occured";
                        }
                    }
                    else
                    {
                        bool createNewDirectConversation =
                            !(NoDirectConversationsWithChannels.Contains(senderParty.ChannelId.ToLower()));

                        // Try to accept
                        messageRouterResult = await messageRouterManager.ConnectAsync(
                            senderParty,
                            partyToAcceptOrReject,
                            createNewDirectConversation);
                    }
                }
                else
                {
                    // Note: Rejecting is OK even if the sender is alreay connected
                    messageRouterResult = messageRouterManager.RejectPendingRequest(partyToAcceptOrReject, senderParty);
                }

                if (messageRouterResult != null)
                {
                    await messageRouterResultHandler.HandleConnectionResultAsync(messageRouterResult);
                }
            }
            else
            {
                errorMessage = $"Failed to find a pending request for user '0': {1}";
            }

            return errorMessage;
        }

        public async Task<bool> RejectAllPendingRequestsAsync(
            MessageRouterManager messageRouterManager, MessageRouterResultHandler messageRouterResultHandler)
        {
            bool wasSuccessful = false;
            IList<Party> pendingRequests = messageRouterManager._routingDataManager.GetPendingRequests();

            if (pendingRequests.Count > 0)
            {
                IList<MessageRouterResult> messageRouterResults = new List<MessageRouterResult>();

                foreach (Party pendingRequest in pendingRequests)
                {
                    messageRouterResults.Add(messageRouterManager.RejectPendingRequest(pendingRequest));
                }

                foreach (MessageRouterResult messageRouterResult in messageRouterResults)
                {
                    await messageRouterResultHandler.HandleConnectionResultAsync(messageRouterResult);
                }

                wasSuccessful = true;
            }

            return wasSuccessful;
        }
    }
}
