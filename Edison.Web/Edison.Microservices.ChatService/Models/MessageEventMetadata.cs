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
    public class MessageEventMetadata
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string ReportType { get; set; }
        public string Message { get; set; }
    }
}
