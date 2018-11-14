using System;
using System.Collections.Generic;

namespace Edison.Common.Messages.Interfaces
{
    public interface IActionCloseUIUpdated : IMessage
    {
        Guid ResponseId { get; set; }
        Guid ActionId { get; set; }
    }
}
