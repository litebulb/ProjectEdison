using Edison.Common.Messages.Interfaces;
using System;

namespace Edison.Common.Messages
{
    public class ResponseUIUpdatedEvent : IResponseUIUpdated
    {
        public Guid ResponseId { get; set; }
    }
}
