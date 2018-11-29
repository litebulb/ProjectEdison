using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class ActionPlanCreationModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public double PrimaryRadius { get; set; }
        public double SecondaryRadius { get; set; }
        public bool AcceptSafeStatus { get; set; }
        public IEnumerable<ActionModel> OpenActions { get; set; }
        public IEnumerable<ActionModel> CloseActions { get; set; }
    }
}
