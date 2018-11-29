using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MassTransit;
using Edison.Core.Common.Models;
using Edison.Core.Interfaces;
using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;

namespace Edison.NotificationHubService.Consumers
{
    /// <summary>
    /// Masstransit consumer that handles notifications sent to consumers
    /// </summary>
    public class NotificationSendEventConsumer : IConsumer<INotificationSendEvent>
    {
        private readonly ILogger<NotificationSendEventConsumer> _logger;
        private readonly INotificationRestService _notificationRestService;

        public NotificationSendEventConsumer(
            ILogger<NotificationSendEventConsumer> logger,
            INotificationRestService notificationRestService)
        {
            _logger = logger;
            _notificationRestService = notificationRestService;
        }

        public async Task Consume(ConsumeContext<INotificationSendEvent> context)
        {
            try
            {
                if (context.Message != null && context.Message as INotificationSendEvent != null && context.Message.Notification != null)
                {
                    NotificationCreationModel notification = context.Message.Notification;
                    _logger.LogDebug($"NotificationSendEventConsumer: Text: '{notification.NotificationText}'.");
                    _logger.LogDebug($"NotificationSendEventConsumer: Status: '{notification.Status}'.");
                    _logger.LogDebug($"NotificationSendEventConsumer: Title: '{notification.Title}'.");
                    _logger.LogDebug($"NotificationSendEventConsumer: User: '{notification.User}'.");
                    DateTime date = DateTime.UtcNow;
                    notification.ResponseId = context.Message.ResponseId;
                    NotificationModel result = await _notificationRestService.SendNotification(notification);
                    if(result != null)
                    {
                        await context.Publish(new EventSagaReceiveResponseActionCallback()
                        {
                            IsCloseAction = context.Message.IsCloseAction,
                            ResponseId = context.Message.ResponseId,
                            ActionId = context.Message.ActionId,
                            Status = ActionStatus.Success
                        });
                        _logger.LogDebug($"NotificationSendEventConsumer: NotificationId: '{result.NotificationId}' published.");
                        return;
                    }
                }
                _logger.LogError("NotificationSendEventConsumer: Invalid Null or Empty Action Notification");
                throw new Exception("Invalid or Null Action Notification");

            }
            catch (Exception e)
            {
                await context.Publish(new EventSagaReceiveResponseActionCallback()
                {
                    IsCloseAction = context.Message.IsCloseAction,
                    ResponseId = context.Message.ResponseId,
                    ActionId = context.Message.ActionId,
                    Status = ActionStatus.Error
                });
                _logger.LogError($"NotificationSendEventConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
