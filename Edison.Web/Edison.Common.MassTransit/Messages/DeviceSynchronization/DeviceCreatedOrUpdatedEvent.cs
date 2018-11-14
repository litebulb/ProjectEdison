using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class DeviceCreatedOrUpdatedEvent : IDeviceCreatedOrUpdated
    {
        public Guid CorrelationId { get; set; }
        public DeviceModel Device { get; set; }
    }
}
