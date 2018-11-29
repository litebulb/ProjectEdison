using Edison.Common.Messages.Interfaces;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages
{
    public class IoTDevicesUpdatedEvent : IIoTDevicesUpdated
    {
        public List<Guid> DeviceIds { get; set; }
    }
}
