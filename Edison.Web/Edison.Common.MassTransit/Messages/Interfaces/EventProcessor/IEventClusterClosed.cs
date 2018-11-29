using Edison.Core.Common.Models;

namespace Edison.Common.Messages.Interfaces
{
    public interface IEventClusterClosed : IMessage
    {
        EventClusterModel EventCluster { get; set; }
    }
}
