using System;
using Edison.Core.Common.Models;
using Edison.Common.Messages.Interfaces;

namespace Edison.Common.Messages
{
    public class EventSagaReceiveResponseActionCallback : IEventSagaReceiveResponseActionCallback
    {
        public Guid ActionCorrelationId { get; set; }
        public Guid ResponseId { get; set; }
        public ActionStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ActionId { get; set; }
    }
}
