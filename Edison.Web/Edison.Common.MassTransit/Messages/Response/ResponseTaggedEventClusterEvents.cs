using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class ResponseTaggedEventClusterEvents : IResponseTaggedEventClusters
    {
        public ResponseModel Response { get; set; }
    }
}
