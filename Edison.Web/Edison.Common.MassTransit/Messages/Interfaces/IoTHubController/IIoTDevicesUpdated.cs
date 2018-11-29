using System;
using System.Collections.Generic;

namespace Edison.Common.Messages.Interfaces
{
    public interface IIoTDevicesUpdated : IMessage
    {
        List<Guid> DeviceIds { get; set; }
    }
}
