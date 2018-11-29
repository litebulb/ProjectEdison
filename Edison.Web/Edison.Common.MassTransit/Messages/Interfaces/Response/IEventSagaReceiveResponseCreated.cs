using Edison.Core.Common.Models;

namespace Edison.Common.Messages.Interfaces
{
    public interface IEventSagaReceiveResponseCreated : IMessage
    {
        ResponseModel ResponseModel { get; set; }
    }
}
