using Edison.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Edison.Core.Interfaces
{
    public interface INotificationRestService
    {
        Task<bool> RegisterDevice(MobileDeviceNotificationHubInstallationModel deviceInstallation);
        Task<NotificationModel> SendNotification(NotificationModel notificationReq);
        Task<IEnumerable<NotificationModel>> GetNotificationsHistory(int pageSize, string continuationToken);
    }
}
