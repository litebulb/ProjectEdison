using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IEventClusterLifespanElapsed : IMessage
    {
        Guid EventClusterId { get; set; }
    }
}
