using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages
{
    public class EventClusterLifespanElapsed : IEventClusterLifespanElapsed
    {
        public Guid EventClusterId { get; set; }
    }
}
