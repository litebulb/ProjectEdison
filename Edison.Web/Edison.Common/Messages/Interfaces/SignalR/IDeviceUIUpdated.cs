using System;
using System.Collections.Generic;

namespace Edison.Common.Messages.Interfaces
{
    public interface IDeviceUIUpdated : IMessage
    {
        Guid CorrelationId { get; set; }
    }
}
