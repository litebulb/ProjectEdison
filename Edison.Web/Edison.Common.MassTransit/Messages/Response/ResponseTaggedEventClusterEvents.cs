using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;

namespace Edison.Common.Messages
{
    public class ResponseTaggedEventClusterEvents : IResponseTaggedEventClusters
    {
        public ResponseModel Response { get; set; }
    }
}
