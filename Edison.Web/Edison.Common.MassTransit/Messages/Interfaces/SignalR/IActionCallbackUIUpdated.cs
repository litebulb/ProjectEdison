using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IActionCallbackUIUpdated : IMessage
    {
        Guid ResponseId { get; set; }
        Guid ActionId { get; set; }
    }
}
