using Edison.Core.Common.Models;

namespace Edison.Common.Messages.Interfaces
{
    public interface IResponseTaggedEventClusters : IMessage
    {
        ResponseModel Response { get; set; }
    }
}
