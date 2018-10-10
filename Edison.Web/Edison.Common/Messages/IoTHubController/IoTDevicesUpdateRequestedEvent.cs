using Edison.Common.Messages.Interfaces;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages
{
    public class IoTDevicesUpdateRequestedEvent : IIoTDevicesUpdateRequested
    {
        public List<Guid> DeviceIds { get; set; }
        public string JsonTags { get; set; }
        public string JsonDesired { get; set; }
        public bool WaitForCompletion { get; set; }
    }
}
