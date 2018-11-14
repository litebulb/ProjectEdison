using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IResponseTagNewEventClusterRequested : IMessage
    {
        Guid EventClusterId { get; set; }
        Geolocation EventClusterGeolocation { get; set; }
    }
}
