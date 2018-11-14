using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages
{
    public class ActionCloseUIUpdatedEvent : IActionCloseUIUpdated
    {
        public Guid ResponseId { get; set; }
        public Guid ActionId { get; set; }
    }
}
