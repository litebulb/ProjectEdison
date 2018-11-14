using Newtonsoft.Json;
using System;

namespace Edison.Mobile.User.Client.Core.Chat
{
    [Serializable]
    public class CommandEndConversation
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
    }
}
