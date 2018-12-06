using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages
{
    public class EventSagaReceiveResponseUpdated : IEventSagaReceiveResponseUpdated
    {
        public ResponseModel Response { get; set; }
        public ActionStatus ActionStatus { get; set; }
    }
}
