using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class DeviceDeletedEvent : IDeviceDeleted
    {
        public Guid DeviceId { get; set; }
    }
}
