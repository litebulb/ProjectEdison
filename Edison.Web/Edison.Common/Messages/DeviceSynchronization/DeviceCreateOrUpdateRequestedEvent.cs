using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class DeviceCreateOrUpdateRequestedEvent : IDeviceCreateOrUpdateRequested
    {
        public Guid DeviceId { get; set; }
        public string ChangeType { get; set; }
        public DateTime Date { get; set; }
        public string Data { get; set; }
    }
}
