using System;
using Newtonsoft.Json;

namespace Edison.ChatService.Models
{
    /// <summary>
    /// Object that represents the channeldata for command EndConversation
    /// </summary>
    [Serializable]
    public class CommandEndConversation
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
    }
}
