using System.Collections.Generic;

namespace Edison.Common.DAO
{
    public class ResponseActionPlanDAOObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public double PrimaryRadius { get; set; }
        public double SecondaryRadius { get; set; }
        public bool AcceptSafeStatus { get; set; }
        public IEnumerable<ResponseActionDAOObject> OpenActions { get; set; }
        public IEnumerable<ResponseActionDAOObject> CloseActions { get; set; }
    }
}
