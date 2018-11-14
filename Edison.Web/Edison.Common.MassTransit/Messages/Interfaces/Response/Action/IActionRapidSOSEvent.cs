using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IActionRapidSOSEvent : IMessage, IActionBaseEvent
    {
        string ServiceType { get; set; }
        string Message { get; set; }
    }
}
