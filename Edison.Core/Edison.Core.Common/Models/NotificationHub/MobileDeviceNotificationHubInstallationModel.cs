using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class MobileDeviceNotificationHubInstallationModel
    {
        public MobileDeviceNotificationHubInstallationModel()
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
