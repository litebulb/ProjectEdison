using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IIoTDeviceDeleted : IMessage
    {
        Guid DeviceId { get; set; }
    }
}
