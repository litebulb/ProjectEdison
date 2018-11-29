using System;
using Newtonsoft.Json;

namespace Edison.ChatService.Models
{
    /// <summary>
    /// Object to indicate when was the last time that a user conversation was read
    /// </summary>
    public class ChatUserReadStatusModel
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }
    }
}
