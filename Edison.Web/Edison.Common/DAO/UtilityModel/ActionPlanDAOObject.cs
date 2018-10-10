using Edison.Core.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Edison.Common.DAO
{
    public class ActionPlanDAOObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public double PrimaryRadius { get; set; }
        public double SecondaryRadius { get; set; }
        public IEnumerable<ActionDAOObject> OpenActions { get; set; }
        public IEnumerable<ActionDAOObject> CloseActions { get; set; }
    }
}
