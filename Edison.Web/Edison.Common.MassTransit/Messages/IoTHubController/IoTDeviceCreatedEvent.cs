using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages
{
    public class IoTDeviceCreatedEvent : IIoTDeviceCreated
    {
        public Guid DeviceId { get; set; }
    }
}
