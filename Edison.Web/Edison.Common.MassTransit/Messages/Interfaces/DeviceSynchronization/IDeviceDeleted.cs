using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IDeviceDeleted : IMessage
    {
        Guid CorrelationId { get; set; }
        Guid DeviceId { get; set; }
    }
}
