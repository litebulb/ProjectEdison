using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;

namespace Edison.Common.Messages
{
    public class NotificationEvent : INotificationEvent
    {
        public NotificationCreationModel Notification { get; set; }
    }
}
