using System;

namespace Edison.Core.Common.Models
{
    public class ActionPlanListModel
    {
        public Guid ActionPlanId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
    }
}
