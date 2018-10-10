using Edison.Common.Chat.Models;
using Edison.Common.Chat.Models.Interface;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Common.Chat.Repositories
{

    [Serializable]
    public abstract class AbstractRoutingDataManager : IChatRoutingDataManagerRepository
    {
        public virtual GlobalTimeProvider GlobalTimeProvider
        {
            get;
            protected set;
        }

#if DEBUG
        protected IList<MessageRouterResult> LastMessageRouterResults
        {
            get;
            set;
        }
#endif
        public AbstractRoutingDataManager()
        {}


        public AbstractRoutingDataManager(GlobalTimeProvider globalTimeProvider = null)
        {
            GlobalTimeProvider = globalTimeProvider ?? new GlobalTimeProvider();
#if DEBUG
            LastMessageRouterResults = new List<MessageRouterResult>();
#endif
        }

        public abstract IList<Party> GetUserParties();
        public abstract IList<Party> GetBotParties();

        public virtual bool AddParty(Party partyToAdd, bool isUser = true)
        {
            if (partyToAdd == null
                || (isUser ?
                    GetUserParties().Contains(partyToAdd)
                    : GetBotParties().Contains(partyToAdd)))
            {
                return false;
            }

            if (!isUser && partyToAdd.ChannelAccount == null)
            {
                throw new NullReferenceException($"Channel account of a bot party ({nameof(partyToAdd.ChannelAccount)}) cannot be null");
            }

            return ExecuteAddPartyAsync(partyToAdd, isUser);
        }

        public virtual IList<MessageRouterResult> RemoveParty(Party partyToRemove)
        {
            List<MessageRouterResult> messageRouterResults = new List<MessageRouterResult>();
            bool wasRemoved = false;

            // Check user and bot parties
            for (int i = 0; i < 2; ++i)
            {
                bool isUser = (i == 0);
                IList<Party> partyList = isUser ? GetUserParties() : GetBotParties();
                IList<Party> partiesToRemove = FindPartiesWithMatchingChannelAccount(partyToRemove, partyList);

                if (partiesToRemove != null)
                {
                    foreach (Party party in partiesToRemove)
                    {
                        wasRemoved = ExecuteRemoveParty(party, isUser);

                        if (wasRemoved)
                        {
                            messageRouterResults.Add(new MessageRouterResult()
                            {
                                Type = MessageRouterResultType.OK
                            });
                        }
                    }
                }
            }

            // Check pending requests
            IList<Party> pendingRequestsToRemove = FindPartiesWithMatchingChannelAccount(partyToRemove, GetPendingRequests());

            foreach (Party pendingRequestToRemove in pendingRequestsToRemove)
            {
                MessageRouterResult removePendingRequestResult = RemovePendingRequest(pendingRequestToRemove);

                if (removePendingRequestResult.Type == MessageRouterResultType.ConnectionRejected)
                {
                    // Pending request was removed
                    wasRemoved = true;

                    messageRouterResults.Add(removePendingRequestResult);
                }
            }

            if (wasRemoved)
            {
                // Check if the party exists in ConnectedParties
                List<Party> keys = new List<Party>();

                foreach (var partyPair in GetConnectedParties())
                {
                    if (partyPair.Key.HasMatchingChannelInformation(partyToRemove)
                        || partyPair.Value.HasMatchingChannelInformation(partyToRemove))
                    {
                        keys.Add(partyPair.Key);
                    }
                }

                foreach (Party key in keys)
                {
                    messageRouterResults.AddRange(DisconnectAsync(key, ConnectionProfileType.Agent));
                }
            }

            if (messageRouterResults.Count == 0)
            {
                messageRouterResults.Add(new MessageRouterResult()
                {
                    Type = MessageRouterResultType.NoActionTaken
                });
            }

            return messageRouterResults;
        }

        public abstract IList<Party> GetAgentParties();

        public virtual bool AddAgentParty(Party agentPartyToAdd)
        {
            if (agentPartyToAdd != null)
            {
                //if (agentPartyToAdd.ChannelAccount != null)
                //{
                //    throw new ArgumentException("Aggregation party cannot contain a channel account");
                //}

                IList<Party> aggregationParties = GetAgentParties();

                if (!aggregationParties.Contains(agentPartyToAdd))
                {
                    return ExecuteAddAgentParty(agentPartyToAdd);
                }
            }

            return false;
        }

        public virtual bool RemoveAgentParty(Party agentPartyToRemove)
        {
            return ExecuteRemoveAgentParty(agentPartyToRemove);
        }

        public abstract IList<Party> GetPendingRequests();

        public abstract Task<PartyEntity> AcceptPendingRequest(string conversationId);

        public abstract Task<bool> AddConversationConnection(ConversationParty conversationOwnerParty, ConversationParty conversationClientParty);

        public abstract Task<bool> RemoveConversationConnection(ConversationParty conversationOwnerParty, ConversationParty conversationClientParty);

        public virtual MessageRouterResult AddPendingRequest(
            Party requestorParty, bool rejectConnectionRequestIfNoAggregationChannel = false)
        {
            AddParty(requestorParty, true); // Make sure the requestor party is in the list of user parties

            MessageRouterResult result = new MessageRouterResult()
            {
                ConversationClientParty = requestorParty
            };

            if (requestorParty != null)
            {
                if (IsAssociatedWithAgent(requestorParty))
                {
                    result.Type = MessageRouterResultType.Error;
                    result.ErrorMessage = $"The given party ({requestorParty.ChannelAccount?.Name}) is associated with aggregation and hence invalid to request a connection";
                }
                else if (GetPendingRequests().Contains(requestorParty))
                {
                    result.Type = MessageRouterResultType.ConnectionAlreadyRequested;
                }
                else
                {
                    if (!GetAgentParties().Any() && rejectConnectionRequestIfNoAggregationChannel)
                    {
                        result.Type = MessageRouterResultType.NoAgentsAvailable;
                    }
                    else
                    {
                        requestorParty.ConnectionRequestTime = GetCurrentGlobalTime();

                        if (ExecuteAddPendingRequest(requestorParty))
                        {
                            result.Type = MessageRouterResultType.ConnectionRequested;
                        }
                        else
                        {
                            result.Type = MessageRouterResultType.Error;
                            result.ErrorMessage = "Failed to add the pending request - this is likely an error caused by the storage implementation";
                        }
                    }
                }
            }
            else
            {
                result.Type = MessageRouterResultType.Error;
                result.ErrorMessage = "The given party instance is null";
            }

            return result;
        }

        public virtual MessageRouterResult RemovePendingRequest(Party requestorParty)
        {
            MessageRouterResult result = new MessageRouterResult()
            {
                ConversationClientParty = requestorParty
            };

            if (GetPendingRequests().Contains(requestorParty))
            {
                if (ExecuteRemovePendingRequest(requestorParty))
                {
                    result.Type = MessageRouterResultType.ConnectionRejected;
                }
                else
                {
                    result.Type = MessageRouterResultType.Error;
                    result.ErrorMessage = "Failed to remove the pending request of the given party";
                }
            }
            else
            {
                result.Type = MessageRouterResultType.Error;
                result.ErrorMessage = "Could not find a pending request for the given party";
            }

            return result;
        }

        public virtual bool IsConnected(Party party, ConnectionProfileType connectionProfile)
        {
            bool isConnected = false;

            if (party != null)
            {
                switch (connectionProfile)
                {
                    case ConnectionProfileType.Client:
                        isConnected = GetConnectedParties().Values.Contains(party);
                        break;
                    case ConnectionProfileType.Agent:
                        isConnected = GetConnectedParties().Keys.Contains(party);
                        break;
                    case ConnectionProfileType.Any:
                        isConnected = (GetConnectedParties().Values.Contains(party) || GetConnectedParties().Keys.Contains(party));
                        break;
                    default:
                        break;
                }
            }

            return isConnected;
        }

        public abstract Dictionary<Party, Party> GetConnectedParties();

        public virtual Party GetConnectedCounterpart(Party partyWhoseCounterpartToFind)
        {
            Party counterparty = null;
            Dictionary<Party, Party> connectedParties = GetConnectedParties();

            if (IsConnected(partyWhoseCounterpartToFind, ConnectionProfileType.Client))
            {
                for (int i = 0; i < connectedParties.Count; ++i)
                {
                    if (connectedParties.Values.ElementAt(i).Equals(partyWhoseCounterpartToFind))
                    {
                        counterparty = connectedParties.Keys.ElementAt(i);
                        break;
                    }
                }
            }
            else if (IsConnected(partyWhoseCounterpartToFind, ConnectionProfileType.Agent))
            {
                connectedParties.TryGetValue(partyWhoseCounterpartToFind, out counterparty);
            }

            return counterparty;
        }

        public virtual MessageRouterResult ConnectAndClearPendingRequestAsync(
            Party conversationOwnerParty, Party conversationClientParty)
        {
            MessageRouterResult result = new MessageRouterResult()
            {
                ConversationAgentParty = conversationOwnerParty,
                ConversationClientParty = conversationClientParty
            };

            if (conversationOwnerParty != null && conversationClientParty != null)
            {
                DateTime connectionStartedTime = GetCurrentGlobalTime();
                conversationClientParty.ResetConnectionRequestTime();
                conversationClientParty.ConnectionEstablishedTime = connectionStartedTime;

                bool wasConnectionAdded =
                     ExecuteAddConnection(conversationOwnerParty, conversationClientParty);

                if (wasConnectionAdded)
                {
                    ExecuteRemovePendingRequest(conversationClientParty);
                    //conversationOwnerParty.ChannelAccount = null;
                    ExecuteRemoveAgentParty(conversationOwnerParty);
                    result.Type = MessageRouterResultType.Connected;
                }
                else
                {
                    result.Type = MessageRouterResultType.Error;
                    result.ErrorMessage =
                        $"Failed to add connection between {conversationOwnerParty} and {conversationClientParty}";
                }
            }
            else
            {
                result.Type = MessageRouterResultType.Error;
                result.ErrorMessage = "Either the owner or the client is missing";
            }

            return result;
        }

        public virtual IList<MessageRouterResult> DisconnectAsync(Party party, ConnectionProfileType connectionProfile)
        {
            IList<MessageRouterResult> messageRouterResults = new List<MessageRouterResult>();

            if (party != null)
            {
                List<Party> keysToRemove = new List<Party>();

                foreach (var partyPair in GetConnectedParties())
                {
                    bool removeThisPair = false;

                    switch (connectionProfile)
                    {
                        case ConnectionProfileType.Client:
                            removeThisPair = partyPair.Value.Equals(party);
                            break;
                        case ConnectionProfileType.Agent:
                            removeThisPair = partyPair.Key.Equals(party);
                            break;
                        case ConnectionProfileType.Any:
                            removeThisPair = (partyPair.Value.Equals(party) || partyPair.Key.Equals(party));
                            break;
                        default:
                            break;
                    }

                    if (removeThisPair)
                    {
                        keysToRemove.Add(partyPair.Key);

                        if (connectionProfile == ConnectionProfileType.Agent)
                        {
                            // Since owner is the key in the dictionary, there can be only one
                            break;
                        }
                    }
                }

                messageRouterResults =  RemoveConnectionsAsync(keysToRemove);
            }

            return messageRouterResults;
        }

        public virtual void DeleteAllAsync()
        {
#if DEBUG
            LastMessageRouterResults.Clear();
#endif
        }

        public virtual bool IsAssociatedWithAgent(Party party)
        {
            IList<Party> agentParties = GetAgentParties();

            return (party != null && agentParties != null && agentParties.Count() > 0
                    && agentParties.Where(agentParty =>
                        agentParty.ConversationAccount.Id == party.ConversationAccount.Id
                        && agentParty.ServiceUrl == party.ServiceUrl
                        && agentParty.ChannelId == party.ChannelId).Count() > 0);
        }

        public virtual string ResolveBotNameInConversation(Party party)
        {
            string botName = null;

            if (party != null)
            {
                Party botParty = FindBotPartyByChannelAndConversation(party.ChannelId, party.ConversationAccount);

                if (botParty != null && botParty.ChannelAccount != null)
                {
                    botName = botParty.ChannelAccount.Name;
                }
            }

            return botName;
        }

        public virtual Party FindExistingUserParty(Party partyToFind)
        {
            Party foundParty = null;

            try
            {
                foundParty = GetUserParties().First(party => partyToFind.Equals(party));
            }
            catch (ArgumentNullException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            return foundParty;
        }

        public virtual Party FindPartyByChannelAccountIdAndConversationId(
            string channelAccountId, string conversationId)
        {
            Party userParty = null;

            try
            {
                userParty = GetUserParties().Single(party =>
                        (party.ChannelAccount.Id.Equals(channelAccountId)
                         && party.ConversationAccount.Id.Equals(conversationId)));
            }
            catch (InvalidOperationException)
            {
            }

            return userParty;
        }

        public virtual Party FindBotPartyByChannelAndConversation(
            string channelId, ConversationAccount conversationAccount)
        {
            Party botParty = null;

            try
            {
                botParty = GetBotParties().Single(party =>
                        (party.ChannelId.Equals(channelId)
                         && party.ConversationAccount.Id.Equals(conversationAccount.Id)));
            }
            catch (InvalidOperationException)
            {
            }

            return botParty;
        }

        public virtual Party FindConnectedPartyByChannel(string channelId, ChannelAccount channelAccount)
        {
            Party foundParty = null;

            try
            {
                foundParty = GetConnectedParties().Keys.Single(party =>
                        (party.ChannelId.Equals(channelId)
                         && party.ChannelAccount != null
                         && party.ChannelAccount.Id.Equals(channelAccount.Id)));
            }
            catch (InvalidOperationException)
            {
            }

            if (foundParty == null)
            {
                try
                {
                    // Not found in keys, try the values
                    foundParty = GetConnectedParties().Values.First(party =>
                            (party.ChannelId.Equals(channelId)
                             && party.ChannelAccount != null
                             && party.ChannelAccount.Id.Equals(channelAccount.Id)));
                }
                catch (InvalidOperationException)
                {
                }
            }

            return foundParty;
        }

        public virtual IList<Party> FindPartiesWithMatchingChannelAccount(Party partyToFind, IList<Party> partyCandidates)
        {
            IList<Party> matchingParties = null;
            IEnumerable<Party> foundParties = null;

            try
            {
                foundParties = partyCandidates.Where(party => party.HasMatchingChannelInformation(partyToFind));
            }
            catch (ArgumentNullException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to find parties: {e.Message}");
            }
            catch (InvalidOperationException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to find parties: {e.Message}");
            }

            if (foundParties != null)
            {
                matchingParties = foundParties.ToArray();
            }

            return matchingParties;
        }

#if DEBUG
        public virtual string ConnectionsToString()
        {
            string parties = string.Empty;

            foreach (KeyValuePair<Party, Party> keyValuePair in GetConnectedParties())
            {
                parties += $"{keyValuePair.Key} -> {keyValuePair.Value}\n\r";
            }

            return parties;
        }

        public virtual string GetLastMessageRouterResults()
        {
            string lastResultsAsString = string.Empty;

            foreach (MessageRouterResult result in LastMessageRouterResults)
            {
                lastResultsAsString += $"{result.ToString()}\n";
            }

            return lastResultsAsString;
        }

        public virtual void AddMessageRouterResult(MessageRouterResult result)
        {
            if (result != null)
            {
                if (LastMessageRouterResults.Count > 9)
                {
                    LastMessageRouterResults.Remove(LastMessageRouterResults.ElementAt(0));
                }

                LastMessageRouterResults.Add(result);
            }
        }

        public virtual void ClearMessageRouterResults()
        {
            LastMessageRouterResults.Clear();
        }
#endif

        protected abstract bool ExecuteAddPartyAsync(Party partyToAdd, bool isUser);

        protected abstract bool ExecuteRemoveParty(Party partyToRemove, bool isUser);

        protected abstract bool ExecuteAddAgentParty(Party agentPartyToAdd);

        protected abstract bool ExecuteRemoveAgentParty(Party agentPartyToRemove);

        protected abstract bool ExecuteAddPendingRequest(Party requestorParty);

        protected abstract bool ExecuteRemovePendingRequest(Party requestorParty);

        public abstract bool ExecuteAddConnection(Party conversationOwnerParty, Party conversationClientParty);

        public abstract bool ExecuteRemoveConnection(Party conversationOwnerParty);

        /// <returns>The current global "now" time.</returns>
        protected virtual DateTime GetCurrentGlobalTime()
        {
            return (GlobalTimeProvider == null) ? DateTime.UtcNow : GlobalTimeProvider.GetCurrentTime();
        }

        protected virtual IList<MessageRouterResult> RemoveConnectionsAsync(IList<Party> conversationOwnerParties)
        {
            IList<MessageRouterResult> messageRouterResults = new List<MessageRouterResult>();

            foreach (Party conversationOwnerParty in conversationOwnerParties)
            {
                Dictionary<Party, Party> connectedParties = GetConnectedParties();
                connectedParties.TryGetValue(conversationOwnerParty, out Party conversationClientParty);

                if (ExecuteRemoveConnection(conversationOwnerParty))
                {
                    conversationOwnerParty.ResetConnectionEstablishedTime();
                    conversationClientParty.ResetConnectionEstablishedTime();

                    messageRouterResults.Add(new MessageRouterResult()
                    {
                        Type = MessageRouterResultType.Disconnected,
                        ConversationAgentParty = conversationOwnerParty,
                        ConversationClientParty = conversationClientParty
                    });
                }
            }

            if (messageRouterResults.Count == 0)
            {
                messageRouterResults.Add(new MessageRouterResult()
                {
                    Type = MessageRouterResultType.NoActionTaken
                });
            }

            return messageRouterResults;
        }
    }
}
