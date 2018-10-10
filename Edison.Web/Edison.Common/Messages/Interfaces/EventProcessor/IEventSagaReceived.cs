using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IEventSagaReceived : IMessage
    {
        Guid DeviceId { get; set; }
        string EventType { get; set; }
        DateTime Date { get; set; }
        string Data { get; set; }
    }
}
