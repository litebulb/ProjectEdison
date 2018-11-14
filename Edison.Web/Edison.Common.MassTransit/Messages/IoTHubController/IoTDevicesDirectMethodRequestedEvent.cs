using Edison.Common.Messages.Interfaces;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages
{
    public class IoTDevicesDirectMethodRequestedEvent : IIoTDevicesDirectMethodRequested
    {
        public List<Guid> DeviceIds { get; set; }
        public string MethodName { get; set; }
        public string MethodPayload { get; set; }
        public bool WaitForCompletion { get; set; }
    }
}
