using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IEventUIUpdateRequested : IMessage
    {
        Guid CorrelationId { get; set; }
        EventClusterUIModel EventClusterUI { get; set; }
    }
}
