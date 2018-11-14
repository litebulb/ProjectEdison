using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class EventUIUpdateRequestedEvent : IEventUIUpdateRequested
    {
        public Guid CorrelationId { get; set; }
        public EventClusterUIModel EventClusterUI { get; set; }
    }
}
