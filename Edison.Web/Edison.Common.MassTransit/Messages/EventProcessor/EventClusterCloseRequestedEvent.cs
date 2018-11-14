using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages
{
    public class EventClusterCloseRequestedEvent : IEventClusterCloseRequested
    {
        public Guid EventClusterId { get; set; }
        public DateTime ClosureDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
