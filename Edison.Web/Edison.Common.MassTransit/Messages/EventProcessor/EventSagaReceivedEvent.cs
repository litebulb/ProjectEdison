using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages
{
    public class EventSagaReceivedEvent : IEventSagaReceived
    {
        public Guid DeviceId { get; set; }
        public string EventType { get; set; }
        public DateTime Date { get; set; }
        public string Data { get; set; }
        public bool CheckBoundary { get; set; }
    }
}
