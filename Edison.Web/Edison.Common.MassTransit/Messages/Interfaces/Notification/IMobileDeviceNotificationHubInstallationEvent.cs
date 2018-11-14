using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages.Interfaces
{
    public interface IMobileDeviceNotificationHubInstallationEvent : IMessage
    {
        string InstallationId { get; set; }
        string Platform { get; set; }
        string PushChannel { get; set; }
        List<string> Tags { get; set; }
        Dictionary<string, NotificationPushTemplateModel> Templates { get; set; }
    }
}
