using System;
using System.Threading.Tasks;
using Android;
using Android.App;
using Edison.Core.Common.Models;
using Edison.Mobile.Common.Network;
using Edison.Mobile.Common.Notifications;
using Android.Support.V4.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Gms.Common;


namespace Edison.Mobile.Android.Common.Notifications
{
    public class NotificationService : INotificationService
    {
        readonly NotificationRestService notificationRestService;
 //       readonly Activity mainActivity;
        readonly string permission = Manifest.Permission.AccessNotificationPolicy;

//        public NotificationService(NotificationRestService notificationRestService, Activity mainActivity)
        public NotificationService(NotificationRestService notificationRestService)
        {
//            this.mainActivity = mainActivity;
            this.notificationRestService = notificationRestService;
        }

        public Task<bool> HasNotificationPrivileges()
        {
 //           return Task.FromResult(ContextCompat.CheckSelfPermission(this.mainActivity.ApplicationContext, permission) == Permission.Granted);
            return Task.FromResult(ContextCompat.CheckSelfPermission(BaseApplication.CurrentActivity?.ApplicationContext, permission) == Permission.Granted);
        }

        public Task<bool> RequestNotificationPrivileges()
        {
            if (ContextCompat.CheckSelfPermission(BaseApplication.CurrentActivity?.ApplicationContext, permission) == Permission.Granted)
            {
                return Task.FromResult(true);
            }

            ActivityCompat.RequestPermissions(BaseApplication.CurrentActivity, new string[] { permission }, 0);
            return Task.FromResult(true);
        }

        public Task<DeviceMobileModel> RegisterForNotifications(DeviceRegistrationModel deviceRegistrationModel)
        {
            return notificationRestService.Register(deviceRegistrationModel);
        }
    }
}
