using Edison.Core.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class ActionPlanUpdateModel
    {
        public Guid ActionPlanId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public IEnumerable<ActionModel> OpenActions { get; set; }
        public IEnumerable<ActionModel> CloseActions { get; set; }
    }
}
