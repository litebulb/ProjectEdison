using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;

namespace Edison.Common.Messages
{
    public class EventSagaReceiveResponseClosed : IEventSagaReceiveResponseClosed
    {
        public ResponseModel Response { get; set; }
    }
}
