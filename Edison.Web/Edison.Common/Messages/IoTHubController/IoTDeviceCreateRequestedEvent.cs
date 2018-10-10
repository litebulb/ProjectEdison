using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages
{
    public class IoTDeviceCreateRequestedEvent : IIoTDeviceCreateRequested
    {
        public Guid DeviceId { get; set; }
        public string JsonTags { get; set; }
        public string JsonDesired { get; set; }
    }
}
