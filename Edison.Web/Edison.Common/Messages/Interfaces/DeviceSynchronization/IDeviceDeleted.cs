using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IDeviceDeleted : IMessage
    {
        Guid DeviceId { get; set; }
    }
}
