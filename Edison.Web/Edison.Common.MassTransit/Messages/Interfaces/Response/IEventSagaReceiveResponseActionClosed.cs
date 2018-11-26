using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages.Interfaces
{
    public interface IEventSagaReceiveResponseActionClosed : IMessage
    {
        Guid ResponseId { get; set; }
        Guid ActionId { get; set; }
        bool IsSuccessful { get; set; }
        bool IsSkipped { get; set; }
        string ErrorMessage { get; set; }
        bool IsCloseAction { get; set; }
    }
}
