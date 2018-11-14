using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages.Interfaces
{
    public interface IActionCloseUIUpdatedRequested : IMessage
    {
        Guid ResponseId { get; set; }
        Guid ActionId { get; set; }
        ActionCloseUIModel ActionCloseModel { get; set; }
    }
}
