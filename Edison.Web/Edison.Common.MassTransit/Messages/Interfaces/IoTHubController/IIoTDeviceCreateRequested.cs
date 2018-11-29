using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IIoTDeviceCreateRequested : IMessage
    {
        Guid DeviceId { get; set; }
        string JsonTags { get; set; }
        string JsonDesired { get; set; }
    }
}
