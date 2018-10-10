using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IEventClusterCreatedOrUpdated : IMessage
    {
        DateTime LastEventDate { get; set; }
        EventClusterModel EventCluster { get; set; }
    }
}
