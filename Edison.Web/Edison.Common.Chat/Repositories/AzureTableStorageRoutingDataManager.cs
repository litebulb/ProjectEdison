using Edison.Common.Chat.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Common.Chat.Repositories
{
#pragma warning disable 1998
    [Serializable]
    public class AzureTableStorageRoutingDataManager : AbstractRoutingDataManager
    {
        protected const string TableNameParties = "Parties";
        protected const string TableNameConnections = "Connections";
        protected const string PartitionKey = "PartitionKey";

        protected CloudTable _partiesTable;
        protected CloudTable _connectionsTable;

      
        public AzureTableStorageRoutingDataManager(string connectionString, GlobalTimeProvider globalTimeProvider = null)
            : base(globalTimeProvider)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("The connection string cannot be null or empty");
            }

            _partiesTable = AzureTableStorageRepository.GetTable(connectionString, TableNameParties);
            _connectionsTable = AzureTableStorageRepository.GetTable(connectionString, TableNameConnections);

            MakeSureTablesExist();
        }

        public override IList<Party> GetUserParties()
        {
            List<PartyEntity> partyEntities = null;

            try
            {
                var parties=_partiesTable.ExecuteQuerySegmentedAsync(new TableQuery<PartyEntity>(), null);

                        //.Where(x => x.PartyEntityType == PartyEntityType.User.ToString()).ToList();
            }
            catch (StorageException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to retrieve the user parties: {e.Message}");
                return new List<Party>();
            }

            return ToPartyList(partyEntities).AsReadOnly();
        }

        public override IList<Party> GetBotParties()
        {
            List<PartyEntity> partyEntities = null;

            try
            {
                partyEntities =
                    _partiesTable.ExecuteQuerySegmentedAsync(new TableQuery<PartyEntity>(),null).GetAwaiter().GetResult()
                        .Where(x => x.PartyEntityType == PartyEntityType.Bot.ToString()).ToList();
            }
            catch (StorageException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to retrieve the bot parties: {e.Message}");
                return new List<Party>();
            }

            return ToPartyList(partyEntities).AsReadOnly();
        }

        public override IList<Party> GetAgentParties()
        {
            List<PartyEntity> partyEntities = null;

            try
            {
                partyEntities =
                    _partiesTable.ExecuteQuerySegmentedAsync(new TableQuery<PartyEntity>(),null).GetAwaiter().GetResult()
                        .Where(x => x.PartyEntityType == PartyEntityType.Aggregation.ToString()).ToList();
            }
            catch (StorageException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to retrieve the aggregation parties: {e.Message}");
                return new List<Party>();
            }

            return ToPartyList(partyEntities).AsReadOnly();
        }

        public override IList<Party> GetPendingRequests()
        {
            List<PartyEntity> partyEntities = null;

            try
            {
                partyEntities =
                    _partiesTable.ExecuteQuerySegmentedAsync(new TableQuery<PartyEntity>(),null).GetAwaiter().GetResult()
                        .Where(x => x.PartyEntityType == PartyEntityType.PendingRequest.ToString()).ToList();
            }
            catch (StorageException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to retrieve the pending requests: {e.Message}");
                return new List<Party>();
            }

            return ToPartyList(partyEntities).AsReadOnly();
        }

        public async override Task<PartyEntity> AcceptPendingRequest(string conversationId)
        {
            List<PartyEntity> partyEntities = null;

            try
            {
                partyEntities =
                    _partiesTable.ExecuteQuerySegmentedAsync(new TableQuery<PartyEntity>(), null).GetAwaiter().GetResult()
                        .Where(x => x.PartyEntityType == PartyEntityType.PendingRequest.ToString() && x.RowKey==conversationId).ToList();

                if (partyEntities.Count > 0)
                {
                    AzureTableStorageRepository.DeleteEntry<PartyEntity>(
                    _partiesTable, partyEntities[0].PartitionKey, partyEntities[0].RowKey);
                    return partyEntities[0];
                }
                
            }
            catch (StorageException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to retrieve the pending requests: {e.Message}");
                return null;
            }

            return null;        
        }

        public override Dictionary<Party, Party> GetConnectedParties()
        {
            Dictionary<Party, Party> connectedParties = new Dictionary<Party, Party>();
            List<ConnectionEntity> connectionEntities = null;

            try
            { 
                connectionEntities =
                    _connectionsTable.ExecuteQuerySegmentedAsync(new TableQuery<ConnectionEntity>(),null).GetAwaiter().GetResult()
                    .ToList();
            }
            catch (StorageException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to retrieve the connected parties: {e.Message}");
                return connectedParties; // Return empty dictionary
            }

            foreach (var connectionEntity in connectionEntities)
            {
                connectedParties.Add(
                    JsonConvert.DeserializeObject<PartyEntity>(connectionEntity.Owner).ToParty(),
                    JsonConvert.DeserializeObject<PartyEntity>(connectionEntity.Client).ToParty());
            }

            return connectedParties;
        }

        public override void DeleteAllAsync()
        {
            base.DeleteAllAsync();

            try
            {
                var partyEntities = _partiesTable.ExecuteQuerySegmentedAsync(new TableQuery<PartyEntity>(),null).GetAwaiter().GetResult();

                foreach (var partyEntity in partyEntities)
                {
                    AzureTableStorageRepository.DeleteEntry<PartyEntity>(
                        _partiesTable, partyEntity.PartitionKey, partyEntity.RowKey);
                }
            }
            catch (StorageException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to delete entries: {e.Message}");
                return;
            }

            var connectionEntities = _connectionsTable.ExecuteQuerySegmentedAsync(new TableQuery<ConnectionEntity>(),null).GetAwaiter().GetResult();

            foreach (var connectionEntity in connectionEntities)
            {
                AzureTableStorageRepository.DeleteEntry<ConnectionEntity>(
                    _connectionsTable, connectionEntity.PartitionKey, connectionEntity.RowKey);
            }

            /*
            try
            {
                _partiesTable.Delete();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"An error occured while trying to delete the parties table: {e.Message}");
            }

            try
            {
                _connectionsTable.Delete();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"An error occured while trying to delete the connections table: {e.Message}");
            }
            */
        }

        protected override bool ExecuteAddPartyAsync(Party partyToAdd, bool isUser)
        {
            return AzureTableStorageRepository.Insert<PartyEntity>(
                _partiesTable,
                new PartyEntity(partyToAdd, isUser ? PartyEntityType.User : PartyEntityType.Bot));
        }

        protected override bool ExecuteRemoveParty(Party partyToRemove, bool isUser)
        {
            return AzureTableStorageRepository.DeleteEntry<PartyEntity>(
                _partiesTable,
                PartyEntity.CreatePartitionKey(partyToRemove, isUser ? PartyEntityType.User : PartyEntityType.Bot),
                PartyEntity.CreateRowKey(partyToRemove));
        }

        protected override bool ExecuteAddAgentParty(Party aggregationPartyToAdd)
        {
            return AzureTableStorageRepository.Insert<PartyEntity>(
                _partiesTable, new PartyEntity(aggregationPartyToAdd, PartyEntityType.Aggregation));
        }

        protected override bool ExecuteRemoveAgentParty(Party aggregationPartyToRemove)
        {
            var partyEntitiesToRemove = GetPartyEntitiesByPropertyNameAndValue(
                PartitionKey,
                PartyEntity.CreatePartitionKey(aggregationPartyToRemove, PartyEntityType.Aggregation))
                    .FirstOrDefault();
            if (partyEntitiesToRemove != null)
            {
                return AzureTableStorageRepository.DeleteEntry<PartyEntity>(
                _partiesTable, partyEntitiesToRemove.PartitionKey, partyEntitiesToRemove.RowKey);
            }
            else
                return false;           
        }

        protected override bool ExecuteAddPendingRequest(Party requestorParty)
        {
            return AzureTableStorageRepository.Insert<PartyEntity>(
                _partiesTable, new PartyEntity(requestorParty, PartyEntityType.PendingRequest));
        }

        protected override bool ExecuteRemovePendingRequest(Party requestorParty)
        {
            return AzureTableStorageRepository.DeleteEntry<PartyEntity>(
                _partiesTable,
                PartyEntity.CreatePartitionKey(requestorParty, PartyEntityType.PendingRequest),
                PartyEntity.CreateRowKey(requestorParty));
        }

        public async override Task<bool> AddConversationConnection(ConversationParty conversationOwnerParty, ConversationParty conversationClientParty)
        {
            return AzureTableStorageRepository.Insert<ConnectionEntity>(_connectionsTable, new ConnectionEntity()
            {
                PartitionKey = conversationClientParty.ConversationAccountId,
                RowKey = conversationOwnerParty.ConversationAccountId,
                Client = JsonConvert.SerializeObject(conversationClientParty),
                Owner = JsonConvert.SerializeObject(conversationOwnerParty)
            });
        }

        public async override Task<bool> RemoveConversationConnection(ConversationParty conversationOwnerParty, ConversationParty conversationClientParty)
        {
            //Dictionary<Party, Party> connectedParties = GetConnectedParties();

            //if (connectedParties != null && connectedParties.Remove(conversationOwnerParty))
            //{
              //  Party conversationClientParty = GetConnectedCounterpart(conversationOwnerParty);

                return AzureTableStorageRepository.DeleteEntry<ConnectionEntity>(
                    _connectionsTable,
                    conversationClientParty.ConversationAccountId,
                    conversationOwnerParty.ConversationAccountId);
            //}

            //return false;
        }

        public override bool ExecuteAddConnection(Party conversationOwnerParty, Party conversationClientParty)
        {
            return AzureTableStorageRepository.Insert<ConnectionEntity>(_connectionsTable, new ConnectionEntity()
            {
                PartitionKey = conversationClientParty.ConversationAccount.Id,
                RowKey = conversationOwnerParty.ConversationAccount.Id,
                Client = JsonConvert.SerializeObject(new PartyEntity(conversationClientParty, PartyEntityType.Client)),
                Owner = JsonConvert.SerializeObject(new PartyEntity(conversationOwnerParty, PartyEntityType.Owner))
            });
        }

        public override bool ExecuteRemoveConnection(Party conversationOwnerParty)
        {
            Dictionary<Party, Party> connectedParties = GetConnectedParties();

            if (connectedParties != null && connectedParties.Remove(conversationOwnerParty))
            {
                Party conversationClientParty = GetConnectedCounterpart(conversationOwnerParty);

                return AzureTableStorageRepository.DeleteEntry<ConnectionEntity>(
                    _connectionsTable,
                    conversationClientParty.ConversationAccount.Id,
                    conversationOwnerParty.ConversationAccount.Id);
            }

            return false;
        }

        /// <summary>
        /// Makes sure the required tables exist.
        /// </summary>
        protected virtual void MakeSureTablesExist()
        {
            /*
            _partiesTable.BeginCreateIfNotExists(OnPartiesTableCreateIfNotExistsFinished, null);
            _connectionsTable.BeginCreateIfNotExists(OnConnectionsTableCreateIfNotExistsFinished, null);
            */

            try
            {
                _partiesTable.CreateIfNotExistsAsync();
                System.Diagnostics.Debug.WriteLine("Parties table created or did already exist");
            }
            catch (StorageException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to create the parties table (perhaps it already exists): {e.Message}");
            }

            try
            {
                _connectionsTable.CreateIfNotExistsAsync();
                System.Diagnostics.Debug.WriteLine("Connections table created or did already exist");
            }
            catch (StorageException e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to create the connections table (perhaps it already exists): {e.Message}");
            }
        }

        protected virtual void OnPartiesTableCreateIfNotExistsFinished(IAsyncResult result)
        {
            if (result == null)
            {
                System.Diagnostics.Debug.WriteLine((result.IsCompleted)
                    ? "Create table operation for parties table completed"
                    : "Create table operation for parties table did not complete");
            }
        }

        protected virtual void OnConnectionsTableCreateIfNotExistsFinished(IAsyncResult result)
        {
            if (result == null)
            {
                System.Diagnostics.Debug.WriteLine((result.IsCompleted)
                    ? "Create table operation for connections table completed"
                    : "Create table operation for connections table did not complete");
            }
        }

        /// <summary>
        /// Resolves the parties in the party (cloud) table by the given property name and value.
        /// </summary>
        /// <param name="propertyName">The property name for the filter.</param>
        /// <param name="value">Party property values to match.</param>
        /// <returns>The party entities in the table matching the given property name and value.</returns>
        protected virtual IEnumerable<PartyEntity> GetPartyEntitiesByPropertyNameAndValue(string propertyName, string value)
        {
            TableQuery<PartyEntity> tableQuery =
                new TableQuery<PartyEntity>()
                    .Where(TableQuery.GenerateFilterCondition(propertyName, QueryComparisons.Equal, value));

            return _partiesTable.ExecuteQuerySegmentedAsync(tableQuery,null).GetAwaiter().GetResult();
        }

        protected virtual List<Party> ToPartyList(IEnumerable<PartyEntity> partyEntities)
        {
            List<Party> partyList = new List<Party>();

            if (partyEntities != null)
            {
                foreach (var partyEntity in partyEntities)
                {
                    partyList.Add(partyEntity.ToParty());
                }
            }
            
            return partyList.ToList();
        }
    }
}
