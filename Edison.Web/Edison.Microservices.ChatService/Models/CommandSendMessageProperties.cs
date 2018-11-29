using System;
using Newtonsoft.Json;
using Edison.Core.Common.Models;

namespace Edison.ChatService.Models
{
    /// <summary>
    /// Object that represents the channeldata for command SendMessage
    /// </summary>
    [Serializable]
    public class CommandSendMessageProperties
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
        [JsonProperty(PropertyName = "from")]
        public ChatUserModel From { get; set; }
        [JsonProperty(PropertyName = "reportType")]
        public Guid? ReportType { get; set; }
    }
}
