using Edison.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Edison.Common.DAO
{
    public class ActionDAOObject
    {
        public Guid ActionId { get; set; }
        public string ActionType { get; set; }
        public int IsActive { get; set; }
        public bool RequiresLocation { get; set; }
        public string Description { get; set; }
        public IDictionary<string, string> Parameters { get; set; }       
    }
}
