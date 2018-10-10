using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Edison.Core.Interfaces
{
    public interface IEventClusterRestService
    {
        Task<EventClusterModel> GetEventCluster(Guid eventClusterId);
        Task<IEnumerable<EventClusterModel>> GetEventClusters();
        Task<IEnumerable<Guid>> GetClustersInRadius(EventClusterGeolocationModel eventClusterGeocodeCenterUpdate);
        Task<EventClusterModel> CreateOrUpdateEventCluster(EventClusterCreationModel eventObj);
        Task<EventClusterModel> CloseEventCluster(EventClusterCloseModel eventObj);
    }
}
