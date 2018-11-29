using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IActionBaseEvent : IMessage
    {
        Guid ActionCorrelationId { get; set; }
        Guid ActionId { get; }
        Guid ResponseId { get; set; }
    }
}
