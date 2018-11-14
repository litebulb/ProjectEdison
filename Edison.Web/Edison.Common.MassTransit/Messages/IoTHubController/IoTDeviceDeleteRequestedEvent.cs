using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages
{
    public class IoTDeviceDeleteRequestedEvent : IIoTDeviceDeleteRequested
    {
        public Guid DeviceId { get; set; }
    }
}
