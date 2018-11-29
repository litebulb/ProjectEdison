using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IIoTDeviceCreated : IMessage
    {
        Guid DeviceId { get; set; }
    }
}
