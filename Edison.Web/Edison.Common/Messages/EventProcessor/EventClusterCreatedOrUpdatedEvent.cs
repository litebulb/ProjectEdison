using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class EventClusterCreatedOrUpdatedEvent : IEventClusterCreatedOrUpdated
    {
        public DateTime LastEventDate { get; set; }
        public EventClusterModel EventCluster { get; set; }
    }
}
