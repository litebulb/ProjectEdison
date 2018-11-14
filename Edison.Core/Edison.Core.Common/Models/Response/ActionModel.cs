using Edison.Core.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class ActionModel : IActionModel
    {
        public Guid ActionId { get; set; }
        public string ActionType { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public Dictionary<string,string> Parameters { get; set; }
    }
}
