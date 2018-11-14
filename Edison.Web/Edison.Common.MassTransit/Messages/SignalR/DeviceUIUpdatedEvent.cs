using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages
{
    public class DeviceUIUpdatedEvent : IDeviceUIUpdated
    {
        public Guid CorrelationId { get; set; }
    }
}
