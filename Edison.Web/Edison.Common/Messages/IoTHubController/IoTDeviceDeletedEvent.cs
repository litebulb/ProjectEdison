using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages
{
    public class IoTDeviceDeletedEvent : IIoTDeviceDeleted
    {
        public Guid DeviceId { get; set; }
    }
}
