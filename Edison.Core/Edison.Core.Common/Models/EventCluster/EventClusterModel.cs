using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class EventClusterModel
    {
        public Guid EventClusterId { get; set; }
        public string EventType { get; set; }
        public EventClusterDeviceModel Device { get; set; }
        public int EventCount { get; set; }
        public IEnumerable<EventModel> Events { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ClosureDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
