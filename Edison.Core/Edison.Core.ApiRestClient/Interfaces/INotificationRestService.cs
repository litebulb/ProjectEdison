using Edison.Core.Common.Models;
using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edison.Core.Interfaces
{
    public interface INotificationRestService
    {
        Task<DeviceMobileModel> RegisterDevice(DeviceRegistrationModel deviceRegistration);
        Task<bool> RemoveDevice(string registrationId);
        Task<NotificationModel> SendNotification(NotificationCreationModel notificationReq);
        Task<IEnumerable<NotificationModel>> GetNotificationsHistory(int pageSize, string continuationToken);
        Task<IEnumerable<NotificationModel>> GetNotificationsHistory(Guid responseId);
        Task<CollectionQueryResult<RegistrationDescription>> GetRegisteredDevices(int pageSize, string continuationToken);
    }
}
