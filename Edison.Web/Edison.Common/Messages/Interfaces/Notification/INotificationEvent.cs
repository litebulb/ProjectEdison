using Edison.Core.Common.Models;
using System;

namespace Edison.Common.Messages.Interfaces
{
    public interface INotificationEvent : IMessage
    {
        NotificationModel Notification { get; set; }
    }
}
