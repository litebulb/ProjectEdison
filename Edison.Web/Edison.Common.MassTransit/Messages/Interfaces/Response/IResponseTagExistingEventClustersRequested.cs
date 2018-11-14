using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IResponseTagExistingEventClustersRequested : IMessage
    {
        Guid ResponseId { get; set; }
        Geolocation ResponseGeolocation { get; set; }
        double Radius { get; set; }
    }
}
