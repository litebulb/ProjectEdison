using System;

namespace Edison.Core.Common.Models
{
    public class EventClusterCloseModel
    {
        public Guid EventClusterId { get; set; }
        public DateTime ClosureDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
