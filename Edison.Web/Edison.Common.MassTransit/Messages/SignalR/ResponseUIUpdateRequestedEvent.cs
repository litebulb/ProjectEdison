using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class ResponseUIUpdateRequestedEvent : IResponseUIUpdateRequested
    {
        public Guid ResponseId { get; set; }
        public ResponseUIModel ResponseUI { get; set; }
    }
}
