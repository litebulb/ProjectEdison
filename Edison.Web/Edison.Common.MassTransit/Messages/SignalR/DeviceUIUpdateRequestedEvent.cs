using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class DeviceUIUpdateRequestedEvent : IDeviceUIUpdateRequested
    {
        public Guid CorrelationId { get; set; }
        public DeviceUIModel DeviceUI { get; set; }
    }
}
