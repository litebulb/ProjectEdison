using System;
using System.Collections.Generic;

namespace Edison.Common.DAO
{
    public class ActionDAOObject
    {
        public string ActionType { get; set; }
        public int IsActive { get; set; }
        public string Description { get; set; }
        public IDictionary<string, string> Parameters { get; set; }       
    }
}
