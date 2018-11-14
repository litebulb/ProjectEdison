using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages.Interfaces
{
    public interface IIoTDevicesDirectMethodLaunched : IMessage
    {
        List<Guid> DeviceIds { get; set; }
    }
}
