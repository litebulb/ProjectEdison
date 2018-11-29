using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IIoTDeviceDeleteRequested : IMessage
    {
        Guid DeviceId { get; set; }
    }
}
