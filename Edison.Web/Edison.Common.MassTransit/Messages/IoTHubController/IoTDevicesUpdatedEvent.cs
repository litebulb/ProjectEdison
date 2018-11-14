using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages
{
    public class IoTDevicesUpdatedEvent : IIoTDevicesUpdated
    {
        public List<Guid> DeviceIds { get; set; }
    }
}
