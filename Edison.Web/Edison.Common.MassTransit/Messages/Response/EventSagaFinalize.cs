using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class EventSagaFinalize : IEventSagaFinalize
    {
        public Guid ResponseId { get; set; }
    }
}
