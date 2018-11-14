using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages
{
    public class EventCloseSagaReceivedEvent : IEventCloseSagaReceived
    {
        public Guid DeviceId { get; set; }
        public string EventType { get; set; }
    }
}
