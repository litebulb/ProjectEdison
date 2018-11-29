using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages
{
    public class DeviceDeletedEvent : IDeviceDeleted
    {
        public Guid CorrelationId { get; set; }
        public Guid DeviceId { get; set; }
    }
}
