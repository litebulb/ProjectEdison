using Edison.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.ChatService.Models
{
    public class ChatUserReadStatusModel
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
        [JsonProperty(PropertyName = "date")]
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Date { get; set; }
    }
}
