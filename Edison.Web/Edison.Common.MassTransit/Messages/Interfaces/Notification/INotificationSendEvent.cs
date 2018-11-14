using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface INotificationSendEvent : IMessage, IActionBaseEvent
    {
        NotificationCreationModel Notification { get; set; }
    }
}
