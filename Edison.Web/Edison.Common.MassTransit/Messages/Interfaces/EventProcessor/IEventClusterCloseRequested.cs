using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IEventClusterCloseRequested : IMessage
    {
        Guid EventClusterId { get; set; }
        DateTime ClosureDate { get; set; }
        DateTime EndDate { get; set; }
    }
}
