using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class EventSagaReceiveResponseCreated : IEventSagaReceiveResponseCreated
    {
        public ResponseModel ResponseModel { get; set; }
    }
}
