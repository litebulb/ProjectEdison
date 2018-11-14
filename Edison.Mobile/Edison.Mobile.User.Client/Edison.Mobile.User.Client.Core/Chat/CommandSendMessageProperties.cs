using Edison.Core.Common.Models;
using Newtonsoft.Json;
using System;

namespace Edison.Mobile.User.Client.Core.Chat
{
    [Serializable]
    public class CommandSendMessageProperties
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "from")]
        public ChatUserModel From { get; set; }

        [JsonProperty(PropertyName = "reportType")]
        public string ReportType { get; set; }
    }
}
