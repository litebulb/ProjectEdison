using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.ChatBot.Client
{
    public class ClientConfig
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string BotId { get; set; }
        public string BotSecret { get; set; }
        public Guid DeviceId { get; set; }
        public string ReportType { get; set; }
        public string DirectLineServiceAPI { get; set; }
    }
}
