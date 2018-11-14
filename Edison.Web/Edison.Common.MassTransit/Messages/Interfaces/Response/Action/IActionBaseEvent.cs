using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IActionBaseEvent : IMessage
    {
        Guid ActionId { get; }
        Guid ResponseId { get; set; }
        bool IsCloseAction { get; set; }
    }
}
