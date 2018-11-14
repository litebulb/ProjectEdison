using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages
{
    public class EventUIUpdatedEvent : IEventUIUpdated
    {
        public Guid CorrelationId { get; set; }
    }
}
