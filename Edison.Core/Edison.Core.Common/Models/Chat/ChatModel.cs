using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class ChatModel
    {
        public string MessageId { get; set; }
        public string ResponseId { get; set; }
        public string SenderUserId { get; set; }
        public Geolocation Geolocation { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> UserIdScope { get; set; }
        public string Message { get; set; }
    }
}
