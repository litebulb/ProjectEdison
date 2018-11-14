using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class NotificationEvent : INotificationEvent
    {
        public NotificationCreationModel Notification { get; set; }
    }
}
