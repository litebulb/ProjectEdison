using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages
{
    public class NotificationSendEvent : INotificationSendEvent
    {
        public Guid ActionCorrelationId { get; set; }
        public NotificationCreationModel Notification { get; set; }
        public bool IsCloseAction { get; set; }
        public Guid ActionId { get; set; }
        public Guid ResponseId { get; set; }
    }
}
