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
    public class CommandReadUserMessages
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
        [JsonProperty(PropertyName = "date")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Date { get; set; }
    }
}
