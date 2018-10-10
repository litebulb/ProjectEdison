using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;

namespace Edison.Common.Messages
{
    public class MobileDeviceNotificationHubInstallationEvent : IMobileDeviceNotificationHubInstallationEvent
    {
        public MobileDeviceNotificationHubInstallationEvent()
        {
            Tags = new List<string>();
            Templates = new Dictionary<string, NotificationPushTemplateModel>();
        }
        public string InstallationId { get; set; }
        public string Platform { get; set; }
        public string PushChannel { get; set; }
        public List<string> Tags { get; set; }
        public Dictionary<string, NotificationPushTemplateModel> Templates { get; set; }
    }
}
