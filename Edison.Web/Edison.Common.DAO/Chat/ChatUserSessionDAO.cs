using Edison.Common.Interfaces;
using Edison.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Edison.Common.DAO
{
    public class ChatUserSessionDAO : IEntityDAO
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string BotId { get; set; }
        public string BotName { get; set; }
        public string ConversationId { get; set; }
        public string ChannelId { get; set; }
        public string ServiceUrl { get; set; }
        public List<ChatUserReadStatusDAOObject> UsersReadStatus { get; set; }
        [JsonProperty(PropertyName = "_etag")]
        public string ETag { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime CreationDate { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime UpdateDate { get; set; }
    }
}
