using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IEventCloseSagaReceived : IMessage
    {
        Guid DeviceId { get; set; }
        string EventType { get; set; }
    }
}
