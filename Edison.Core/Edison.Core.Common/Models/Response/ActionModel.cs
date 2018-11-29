using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class ActionModel
    {
        public string ActionType { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public Dictionary<string,string> Parameters { get; set; }
    }
}
