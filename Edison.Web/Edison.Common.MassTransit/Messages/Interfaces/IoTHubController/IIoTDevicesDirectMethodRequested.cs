using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages.Interfaces
{
    public interface IIoTDevicesDirectMethodRequested : IMessage
    {
        List<Guid> DeviceIds { get; set; }
        string MethodName { get; set; }
        string MethodPayload { get; set; }
        bool WaitForCompletion { get; set; }
    }
}
