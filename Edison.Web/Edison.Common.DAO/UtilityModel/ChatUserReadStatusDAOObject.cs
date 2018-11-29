using Edison.Core.Common;
using Newtonsoft.Json;
using System;

namespace Edison.Common.DAO
{
    public class ChatUserReadStatusDAOObject
    {
        public string UserId { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime Date { get; set; }
    }
}
