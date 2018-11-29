using System;
using System.Collections.Generic;

namespace Edison.Common.Messages.Interfaces
{
    public interface IIoTDevicesUpdateRequested : IMessage
    {
        List<Guid> DeviceIds { get; set; }
        string JsonTags { get; set; }
        string JsonDesired { get; set; }
        bool WaitForCompletion { get; set; }
    }
}
