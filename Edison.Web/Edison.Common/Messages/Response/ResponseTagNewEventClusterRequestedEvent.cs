using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class ResponseTagNewEventClusterRequestedEvent : IResponseTagNewEventClusterRequested
    {
        public Guid EventClusterId { get; set; }
        public Geolocation EventClusterGeolocation { get; set; }
    }
}
