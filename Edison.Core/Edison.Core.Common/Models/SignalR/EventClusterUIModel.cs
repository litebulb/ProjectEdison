using Edison.Core.Common.Interfaces;

namespace Edison.Core.Common.Models
{
    public class EventClusterUIModel : IUIUpdateSignalR
    {
        public string UpdateType { get; set; }
        public EventClusterModel EventCluster { get; set; }
    }
}
