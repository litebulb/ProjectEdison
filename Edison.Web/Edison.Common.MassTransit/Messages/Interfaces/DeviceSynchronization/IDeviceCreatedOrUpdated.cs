using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IDeviceCreatedOrUpdated : IMessage
    {
        Guid CorrelationId { get; set; }
        bool NotifyUI { get; set; }
        DeviceModel Device { get; set; }
    }
}
