using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class ResponseTagExistingEventClustersRequestedEvent : IResponseTagExistingEventClustersRequested
    {
        public Guid ResponseId { get; set; }
        public Geolocation ResponseGeolocation { get; set; }
        public double Radius { get; set; }
    }
}
