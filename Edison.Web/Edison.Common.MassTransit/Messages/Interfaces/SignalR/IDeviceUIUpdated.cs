using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IDeviceUIUpdated : IMessage
    {
        Guid CorrelationId { get; set; }
    }
}
