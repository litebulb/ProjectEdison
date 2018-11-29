using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class ResponseActionPlanModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public double PrimaryRadius { get; set; }
        public double SecondaryRadius { get; set; }
        public bool AcceptSafeStatus { get; set; }
        public virtual IEnumerable<ResponseActionModel> OpenActions { get; set; }
        public virtual IEnumerable<ResponseActionModel> CloseActions { get; set; }
    }
}
