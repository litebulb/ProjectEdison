using System;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Common.Network;
using Edison.Mobile.Common.Notifications;
using Foundation;
using UIKit;
using UserNotifications;

namespace Edison.Mobile.iOS.Common.Notifications
{
    public class NotificationService : NSObject, INotificationService
    {
        readonly NotificationRestService notificationRestService;

        public Guid DeviceId { get; set; }

        public NotificationService(NotificationRestService notificationRestService)
        {
            this.notificationRestService = notificationRestService;
        }

        public Task<bool> HasNotificationPrivileges()
        {
            var taskSource = new TaskCompletionSource<bool>();

            UNUserNotificationCenter.Current.GetNotificationSettings(settings => taskSource.TrySetResult(settings.AuthorizationStatus == UNAuthorizationStatus.Authorized));

            return taskSource.Task;
        }

        public Task<bool> RequestNotificationPrivileges()
        {
            var taskSource = new TaskCompletionSource<bool>();

            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge, (granted, error) =>
            {
                taskSource.TrySetResult(granted);
                InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
            });

            return taskSource.Task;
        }

        public async Task<DeviceMobileModel> RegisterForNotifications(DeviceRegistrationModel deviceRegistrationModel)
        {
            var deviceMobileModel = await notificationRestService.Register(deviceRegistrationModel);

            DeviceId = deviceMobileModel?.DeviceId ?? DeviceId;

            return deviceMobileModel;
        }
    }
}
