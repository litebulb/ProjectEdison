using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Common.Chat.Models.Interface
{
    public interface IChatRoutingDataManagerRepository
    {
        #region CRUD methods

        IList<Party> GetUserParties();

        IList<Party> GetBotParties();

        bool AddParty(Party partyToAdd, bool isUser = true);

        IList<MessageRouterResult> RemoveParty(Party partyToRemove);

        IList<Party> GetAgentParties();

        bool AddAgentParty(Party agentPartyToAdd);

        bool RemoveAgentParty(Party agentPartyToRemove);

        IList<Party> GetPendingRequests();

        Task<PartyEntity> AcceptPendingRequest(string conversationId);

        MessageRouterResult AddPendingRequest(
            Party requestorParty, bool rejectConnectionRequestIfNoAgentChannel = false);

        MessageRouterResult RemovePendingRequest(Party requestorParty);

        bool IsConnected(Party party, ConnectionProfileType connectionProfile);

        Dictionary<Party, Party> GetConnectedParties();

        Party GetConnectedCounterpart(Party partyWhoseCounterpartToFind);

        MessageRouterResult ConnectAndClearPendingRequestAsync(Party conversationOwnerParty, Party conversationClientParty);

        IList<MessageRouterResult> DisconnectAsync(Party party, ConnectionProfileType connectionProfile);

        Task<bool> AddConversationConnection(ConversationParty conversationOwnerParty, ConversationParty conversationClientParty);

        Task<bool> RemoveConversationConnection(ConversationParty conversationOwnerParty, ConversationParty conversationClientParty);

        void DeleteAllAsync();
        #endregion

        #region Utility methods

        bool IsAssociatedWithAgent(Party party);

        string ResolveBotNameInConversation(Party party);

        Party FindExistingUserParty(Party partyToFind);

        Party FindPartyByChannelAccountIdAndConversationId(string channelAccountId, string conversationId);

        Party FindBotPartyByChannelAndConversation(string channelId, ConversationAccount conversationAccount);

        Party FindConnectedPartyByChannel(string channelId, ChannelAccount channelAccount);

        IList<Party> FindPartiesWithMatchingChannelAccount(Party partyToFind, IList<Party> partyCandidates);
        #endregion

        #region Methods for debugging
#if DEBUG
        /// <returns>The connections (parties in conversation) as a string.
        /// Will return an empty string, if no connections exist.</returns>
        string ConnectionsToString();

        string GetLastMessageRouterResults();

        void AddMessageRouterResult(MessageRouterResult result);

        void ClearMessageRouterResults();
#endif
        #endregion
    }
}
