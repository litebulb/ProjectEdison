using System;
using System.Threading.Tasks;
using Edison.Core.Common.Models;

namespace Edison.Mobile.Common.Notifications
{
    public interface INotificationService
    {
        Task<bool> HasNotificationPrivileges();
        Task<bool> RequestNotificationPrivileges();
        Task<DeviceMobileModel> RegisterForNotifications(DeviceRegistrationModel deviceRegistrationModel);
    }
}
