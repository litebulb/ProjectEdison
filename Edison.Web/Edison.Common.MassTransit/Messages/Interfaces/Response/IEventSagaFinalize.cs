using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages.Interfaces
{
    public interface IEventSagaFinalize : IMessage
    {
        Guid ResponseId { get; set; }
    }
}
