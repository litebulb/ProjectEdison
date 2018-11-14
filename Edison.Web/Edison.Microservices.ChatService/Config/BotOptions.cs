using Edison.Core.Common.Models;
using Edison.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.ChatService.Config
{
    public class BotOptions
    {
        public string MicrosoftAppId { get; set; }
        public string MicrosoftAppPassword { get; set; }
        public string BotSecret { get; set; }
        public string AdminChannel { get; set; }
        public Guid EmergencyActionPlanId { get; set; }
        public List<string> ClaimsId { get; set; }
        public List<string> ClaimsFirstName { get; set; }
        public List<string> ClaimsLastName { get; set; }
        public List<string> ClaimsRole { get; set; }
        public List<string> ClaimsRoleAdminValues { get; set; }
        public RestServiceOptions RestService { get; set; }
    }
}
