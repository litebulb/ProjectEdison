using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IActionCallbackUIUpdatedRequested : IMessage
    {
        Guid ResponseId { get; set; }
        Guid ActionId { get; set; }
        ActionCallbackUIModel ActionCloseModel { get; set; }
    }
}
