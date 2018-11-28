using System;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Common.Network;
using Edison.Mobile.Common.Notifications;

namespace Edison.Mobile.Android.Common.Notifications
{
    public class NotificationService : INotificationService
    {
        readonly NotificationRestService notificationRestService;

        public NotificationService(NotificationRestService notificationRestService)
        {
            this.notificationRestService = notificationRestService;
        }

        public Task<bool> HasNotificationPrivileges()
        {
            return Task.FromResult<bool>(true);
        }

        public Task<bool> RequestNotificationPrivileges()
        {
            return Task.FromResult<bool>(true);
        }

        public Task<bool> RegisterForNotifications(DeviceRegistrationModel deviceRegistrationModel)
        {
            return notificationRestService.Register(deviceRegistrationModel);
        }
    }
}
