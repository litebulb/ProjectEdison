using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class ActionCallbackUIUpdatedRequestedEvent : IActionCallbackUIUpdatedRequested
    {
        public Guid ResponseId { get; set; }
        public Guid ActionId { get; set; }
        public ActionCallbackUIModel ActionCloseModel { get; set; }
    }
}
