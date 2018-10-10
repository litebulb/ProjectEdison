using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface IActionRapidSOSEvent : IMessage
    {
        Guid ActionId { get; }
        string ServiceType { get; set; }
        string Message { get; set; }
    }
}
