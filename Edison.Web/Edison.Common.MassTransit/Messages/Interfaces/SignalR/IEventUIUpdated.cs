using System;
using System.Collections.Generic;

namespace Edison.Common.Messages.Interfaces
{
    public interface IEventUIUpdated : IMessage
    {
        Guid CorrelationId { get; set; }
    }
}
