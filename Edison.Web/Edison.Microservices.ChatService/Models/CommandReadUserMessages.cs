using System;
using Newtonsoft.Json;

namespace Edison.ChatService.Models
{
    /// <summary>
    /// Object that represents the channeldata for command ReadUserMessages
    /// </summary>
    [Serializable]
    public class CommandReadUserMessages
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
        [JsonProperty(PropertyName = "date")]
        //[JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Date { get; set; }
    }
}
