using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Core.Common.Models
{
    public class ChatUserTokenContext
    {
        public string Token { get; set; }
        public int? ExpiresIn { get; set; }
        public string ConversationId { get; set; }
        public ChatUserModel UserContext { get; set; }
    }
}
