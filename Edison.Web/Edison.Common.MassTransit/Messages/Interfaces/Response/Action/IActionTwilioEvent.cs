using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IActionTwilioEvent : IMessage, IActionBaseEvent
    {
        string Message { get; set; }
    }
}
