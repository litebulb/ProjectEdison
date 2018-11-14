using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;

namespace Edison.Common.Messages
{
    public class EventClusterClosedEvent : IEventClusterClosed
    {
        public EventClusterModel EventCluster { get; set; }
    }
}
