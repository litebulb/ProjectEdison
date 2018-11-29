using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;

namespace Edison.Common.Messages
{
    public class EventSagaReceiveResponseCreated : IEventSagaReceiveResponseCreated
    {
        public ResponseModel ResponseModel { get; set; }
    }
}
