using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Common.Chat.Models
{
    public class ConversationParty
    {
        [JsonProperty(PropertyName = "serviceUrl")]
        public string ServiceUrl { get; set; }

        [JsonProperty(PropertyName = "channelId")]
        public string ChannelId { get; set; }

        [JsonProperty(PropertyName = "channelAccountId")]
        public string ChannelAccountId { get; set; }

        [JsonProperty(PropertyName = "channelAccountName")]
        public string ChannelAccountName { get; set; }


        [JsonProperty(PropertyName = "conversationAccountId")]
        public string ConversationAccountId { get; set; }

        [JsonProperty(PropertyName = "conversationAccountName")]
        public string ConversationAccountName { get; set; }

        [JsonProperty(PropertyName = "partyEntityType")]
        public string PartyEntityType { get; set; }

        [JsonProperty(PropertyName = "connectionRequestTime")]
        public string ConnectionRequestTime { get; set; }

        [JsonProperty(PropertyName = "connectionEstablishedTime")]
        public string ConnectionEstablishedTime { get; set; }

        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey { get; set; }

        [JsonProperty(PropertyName = "rowKey")]
        public string RowKey { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty(PropertyName = "eTag")]
        public string ETag { get; set; }

    }
}
