using Edison.Core.Common.Models;

namespace Edison.Common.Messages.Interfaces
{
    public interface INotificationEvent : IMessage
    {
        NotificationCreationModel Notification { get; set; }
    }
}
