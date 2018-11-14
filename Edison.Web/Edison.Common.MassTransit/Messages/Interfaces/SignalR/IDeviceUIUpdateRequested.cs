using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IDeviceUIUpdateRequested : IMessage
    {
        Guid CorrelationId { get; set; }
        DeviceUIModel DeviceUI { get; set; }
    }
}
