using System;
using Edison.Core.Common.Models;


namespace Edison.Common.Messages.Interfaces
{
    public interface IEventSagaReceiveResponseActionCallback : IMessage
    {
        Guid ActionCorrelationId { get; set; }
        Guid ResponseId { get; set; }
        Guid ActionId { get; set; }
        ActionStatus Status { get; set; }
        string ErrorMessage { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
    }
}
