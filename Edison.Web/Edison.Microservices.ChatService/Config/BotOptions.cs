using Edison.Core.Config;
using System;
using System.Collections.Generic;

namespace Edison.ChatService.Config
{
    public class BotOptions
    {
        public string MicrosoftAppId { get; set; }
        public string MicrosoftAppPassword { get; set; }
        public string BotSecret { get; set; }
        public string AdminChannel { get; set; }
        public List<string> ClaimsId { get; set; }
        public List<string> ClaimsFirstName { get; set; }
        public List<string> ClaimsLastName { get; set; }
        public List<string> ClaimsRole { get; set; }
        public List<string> ClaimsRoleAdminValues { get; set; }
        public RestServiceOptions RestService { get; set; }
    }
}
