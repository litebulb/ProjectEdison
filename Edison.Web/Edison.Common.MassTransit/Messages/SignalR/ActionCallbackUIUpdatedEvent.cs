using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages
{
    public class ActionCallbackUIUpdatedEvent : IActionCallbackUIUpdated
    {
        public Guid ResponseId { get; set; }
        public Guid ActionId { get; set; }
    }
}
