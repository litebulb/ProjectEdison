using Edison.Core.Common.Models;

namespace Edison.Common.Messages.Interfaces
{
    public interface IEventSagaReceiveResponseClosed : IMessage
    {
        ResponseModel ResponseModel { get; set; }
    }
}
