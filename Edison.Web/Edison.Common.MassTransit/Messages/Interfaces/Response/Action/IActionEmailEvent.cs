using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IActionEmailEvent : IMessage, IActionBaseEvent
    {
        string Subject { get; set; }
        string ToLine { get; set; }
        string CCLine { get; set; }
        string Body { get; set; }
    }
}
