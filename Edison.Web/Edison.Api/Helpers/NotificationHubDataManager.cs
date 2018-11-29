using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Edison.Core.Common.Models;
using Edison.Common.Interfaces;
using Edison.Common.DAO;

namespace Edison.Api.Helpers
{
    /// <summary>
    /// Manager for the Notifications repository
    /// </summary>
    public class NotificationHubDataManager
    {
        private readonly IMapper _mapper;
        private readonly ICosmosDBRepository<NotificationDAO> _repoNotifications;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public NotificationHubDataManager(
            IMapper mapper,
            ICosmosDBRepository<NotificationDAO> repoNotifications)
        {
            _mapper = mapper;
            _repoNotifications = repoNotifications;
        }

        /// <summary>
        /// Create a notification
        /// </summary>
        /// <param name="notification">NotificationCreationModel</param>
        /// <returns>NotificationModel</returns>
        public async Task<NotificationModel> CreateNotification(NotificationCreationModel notification)
        {
            var date = DateTime.UtcNow;

            NotificationDAO notificationDao = new NotificationDAO()
            {
                NotificationText = notification.NotificationText,
                CreationDate = date,
                UpdateDate = date,
                ResponseId = notification.ResponseId.ToString(),
                Status = notification.Status,
                Tags = notification.Tags,
                Title = notification.Title,
                User = notification.User
            };

            notificationDao.Id = await _repoNotifications.CreateItemAsync(notificationDao);
            if (_repoNotifications.IsDocumentKeyNull(notificationDao))
                throw new Exception($"An error occured when creating a new notification");

            NotificationModel output = _mapper.Map<NotificationModel>(notificationDao);
            return output;
        }

        /// <summary>
        /// Get notifications
        /// </summary>
        /// <param name="pageSize">Size of a result page</param>
        /// <param name="continuationToken">Continuation token</param>
        /// <returns>List of notifications</returns>
        public async Task<IEnumerable<NotificationModel>> GetNotifications(int pageSize, string continuationToken)
        {
            var notifications = await _repoNotifications.GetItemsPagingAsync(pageSize, continuationToken);
            IEnumerable<NotificationModel> output = _mapper.Map<IEnumerable<NotificationModel>>(notifications.List);
            return output;
        }

        /// <summary>
        /// Get notifications by Response Id
        /// </summary>
        /// <param name="responseId">Response Id</param>
        /// <returns>List of notifications</returns>
        public async Task<IEnumerable<NotificationModel>> GetNotifications(Guid responseId)
        {
            var notifications = await _repoNotifications.GetItemsAsync(p => p.ResponseId == responseId.ToString(),
                p => new NotificationDAO()
                {
                    Id = p.Id,
                    CreationDate = p.CreationDate,
                    NotificationText = p.NotificationText,
                    ResponseId = p.ResponseId,
                    Status = p.Status,
                    Tags = p.Tags,
                    Title = p.Title,
                    UpdateDate = p.UpdateDate,
                    User = p.User
                }
                );
            IEnumerable<NotificationModel> output = _mapper.Map<IEnumerable<NotificationModel>>(notifications);
            return output;
        }
    }
}
