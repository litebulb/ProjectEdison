using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IResponseTaggedEventClusters : IMessage
    {
        ResponseModel Response { get; set; }
    }
}
