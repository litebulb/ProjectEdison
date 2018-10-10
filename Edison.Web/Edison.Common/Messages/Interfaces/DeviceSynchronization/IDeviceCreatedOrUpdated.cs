using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IDeviceCreatedOrUpdated : IMessage
    {
        DeviceModel Device { get; set; }
    }
}
