﻿using System;
using System.Collections.Generic;
using Microsoft.Bot.Connector;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Underscore.Bot.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Underscore.Bot.MessageRouting.MessageRouting.DataStore.Azure;
using System.Diagnostics;

namespace Underscore.Bot.MessageRouting.DataStore.Azure
{
    /// <summary>
    /// Routing data manager that stores the data in Azure Table storage services.
    /// Caching policy: If the local query finds nothing, update the data from the storage.
    /// See IRoutingDataManager for general documentation of properties and methods.
    /// 
    /// NOTE: DO NOT USE THIS CLASS - THIS IS NOT FAR FROM A PLACEHOLDER CURRENTLY
    /// 
    /// See also Get started with Azure Table storage using .NET article:
    /// https://docs.microsoft.com/en-us/azure/storage/storage-dotnet-how-to-use-tables
    /// </summary>
    public class AzureTableStorageRoutingDataManager : AzureStorageBase, IRoutingDataManager
    {
        public const string StorageConnectionStringKey = "AzureTableStorageConnectionString";

        // See https://docs.microsoft.com/fi-fi/rest/api/storageservices/understanding-the-table-service-data-model
        protected const string UserPartiesTableKey          = "userparties";
        protected const string BotPartiesTableKey           = "botparties";
        protected const string AggregationPartiesTableKey   = "aggregationparties";
        protected const string PendingRequestsTableKey      = "pendingrequests";
        protected const string ConnectedPartiesTableKey     = "connectedparties";
#if DEBUG
        protected const string MessageRouterResultsTableKey = "messagerouterresults";
#endif

        protected CloudTableClient _cloudTableClient;


        public AzureTableStorageRoutingDataManager()
        {
            var connectionString = ConfigurationManager.AppSettings[StorageConnectionStringKey];
            if(string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException($"Failed to find connection string in app settings (with key \"{StorageConnectionStringKey}\" nor was non-null string given to constructor");

            Debug.WriteLine($"Azure table storage connection string: '{connectionString}'");
            InitializeWithConnectionString(connectionString);
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connectionString">The connection string for the Azure Table Storage.
        /// If the value is null, the constructor will look for the connection string in the app settings.</param>
        public AzureTableStorageRoutingDataManager(string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConfigurationManager.AppSettings[StorageConnectionStringKey];
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException($"Failed to find connection string in app settings (with key \"{StorageConnectionStringKey}\" nor was non-null string given to constructor");
            }

            System.Diagnostics.Debug.WriteLine($"Azure Table Storage connection string: \"{connectionString}\"");

            InitializeWithConnectionString(connectionString);
        }

        public IList<Party> GetUserParties()
        {
            return GetParties(UserPartiesTableKey);
        }

        public IList<Party> GetBotParties()
        {
            return GetParties(BotPartiesTableKey);
        }

        public bool AddParty(Party newParty, bool isUser = true)
        {
            return AddParty(isUser ? UserPartiesTableKey : BotPartiesTableKey, newParty);
        }

        public bool AddParty(string serviceUrl, string channelId,
            ChannelAccount channelAccount, ConversationAccount conversationAccount, bool isUser = true)
        {
            Party newParty = new PartyWithTimestamps(serviceUrl, channelId, channelAccount, conversationAccount);
            return AddParty(newParty, isUser);
        }

        public MessageRouterResult ConnectAndClearPendingRequest(Party conversationOwnerParty, Party conversationClientParty)
        {
            throw new NotImplementedException();
        }

        public MessageRouterResult AddPendingRequest(Party party)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll()
        {
            throw new NotImplementedException();
        }

        public string ConnectionsToString()
        {
            throw new NotImplementedException();
        }

        public Party FindBotPartyByChannelAndConversation(string channelId, ConversationAccount conversationAccount)
        {
            throw new NotImplementedException();
        }

        public Party FindConnectedPartyByChannel(string channelId, ChannelAccount channelAccount)
        {
            throw new NotImplementedException();
        }

        public Party FindExistingUserParty(Party partyToFind)
        {
            throw new NotImplementedException();
        }

        public IList<Party> FindPartiesWithMatchingChannelAccount(Party partyToFind, IList<Party> parties)
        {
            throw new NotImplementedException();
        }

        public Party FindPartyByChannelAccountIdAndConversationId(string channelAccountId, string conversationId)
        {
            throw new NotImplementedException();
        }

        public IList<Party> GetAggregationParties()
        {
            throw new NotImplementedException();
        }

        public bool AddAggregationParty(Party party)
        {
            throw new NotImplementedException();
        }

        public Party GetConnectedCounterpart(Party partyWhoseCounterpartToFind)
        {
            throw new NotImplementedException();
        }

        public IList<Party> GetPendingRequests()
        {
            throw new NotImplementedException();
        }

        public bool IsAssociatedWithAggregation(Party party)
        {
            throw new NotImplementedException();
        }

        public bool IsConnected(Party party, ConnectionProfile connectionProfile)
        {
            throw new NotImplementedException();
        }

        public bool RemoveAggregationParty(Party party)
        {
            throw new NotImplementedException();
        }

        public IList<MessageRouterResult> Disconnect(Party party, ConnectionProfile connectionProfile)
        {
            throw new NotImplementedException();
        }

        public IList<MessageRouterResult> RemoveParty(Party partyToRemove)
        {
            throw new NotImplementedException();
        }

        public bool RemovePendingRequest(Party party)
        {
            throw new NotImplementedException();
        }

        public string ResolveBotNameInConversation(Party party)
        {
            throw new NotImplementedException();
        }

#if DEBUG
        public string GetLastMessageRouterResults()
        {
            throw new NotImplementedException();
        }

        public void AddMessageRouterResult(MessageRouterResult result)
        {
            throw new NotImplementedException();
        }
#endif

        /// <summary>
        /// Extracts the Party instances from the given collection.
        /// </summary>
        /// <param name="partyTableEntities">A container of PartyTableEntity instances.</param>
        /// <returns>A list of Party instances.</returns>
        protected IList<Party> PartyTableEntitiesToPartyList(IEnumerable<PartyTableEntity> partyTableEntities)
        {
            IList<Party> parties = new List<Party>();

            foreach (PartyTableEntity partyTableEntity in partyTableEntities)
            {
                parties.Add(partyTableEntity.Party);
            }

            return parties;
        }

        /// <summary>
        /// Returns the parties from the table with the given key.
        /// If the table with given key does not exist, one is created.
        /// </summary>
        /// <param name="tableKey">The table key.</param>
        /// <returns>A list of Party instances in the table.</returns>
        protected IList<Party> GetParties(string tableKey)
        {
            var partiesTable = GetOrCreateCloudTable(tableKey);
            if (partiesTable == null)
                return new List<Party>();

            var partyEntities = GetAll<PartyTableEntity>(partiesTable);

            if (!partyEntities.Any())
                return new List<Party>();

            return partyEntities.Select(e => e.Party).ToList();
        }


        /// <param name="tableKey"></param>
        /// <param name="party"></param>
        /// <returns></returns>
        protected bool AddParty(string tableKey, Party party)
        {
            if (string.IsNullOrEmpty(tableKey))
                throw new ArgumentException("message", nameof(tableKey));

            if (party == null)
                throw new ArgumentNullException(nameof(party));

            var partyTableEntity = new PartyTableEntity(party);
            var cloudTable       = GetOrCreateCloudTable(tableKey);

            if (cloudTable == null)
                return false;

            return InsertEntity(cloudTable, partyTableEntity);
        }
    }
}