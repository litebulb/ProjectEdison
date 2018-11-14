using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages
{
    public class EventSagaReceiveResponseActionClosed : IEventSagaReceiveResponseActionClosed
    {
        public EventSagaReceiveResponseActionClosed(bool _isCloseAction = false)
        {
            IsCloseAction = _isCloseAction;
        }
        public Guid ResponseId { get; set; }
        public bool IsSuccessful { get; set; }
        public bool IsCloseAction { get; set; }
        public Guid ActionId { get; set; }
    }
}
