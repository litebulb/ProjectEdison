using Edison.Core.Common;
using Edison.Core.Common.Models;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.ChatService.Models
{
    [Serializable]
    public class CommandSendMessageProperties
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
        [JsonProperty(PropertyName = "from")]
        public ChatUserModel From { get; set; }
        //[JsonProperty(PropertyName = "to")]
        //public ChatUser To { get; set; }
        [JsonProperty(PropertyName = "reportType")]
        public Guid? ReportType { get; set; }
    }
}
