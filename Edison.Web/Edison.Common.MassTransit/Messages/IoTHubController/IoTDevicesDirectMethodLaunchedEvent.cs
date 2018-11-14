using Edison.Common.Messages.Interfaces;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages
{
    public class IoTDevicesDirectMethodLaunchedEvent : IIoTDevicesDirectMethodLaunched
    {
        public List<Guid> DeviceIds { get; set; }
    }
}
