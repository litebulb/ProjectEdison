using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class EventSagaReceiveResponseClosed : IEventSagaReceiveResponseClosed
    {
        public ResponseModel ResponseModel { get; set; }
    }
}
