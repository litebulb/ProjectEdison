using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MassTransit;
using Edison.Core.Interfaces;
using Edison.Core.Common.Models;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;

namespace Edison.ResponseService.Consumers
{
    /// <summary>
    /// Masstransit consumer that handles the notification action from a response
    /// </summary>
    public class ResponseActionNotificationEventConsumer : ResponseActionBaseConsumer, IConsumer<IActionNotificationEvent>
    {
        private readonly INotificationRestService _notificationRestService;

        public ResponseActionNotificationEventConsumer(IResponseRestService responseRestService,
            INotificationRestService notificationRestService,
            ILogger<ResponseActionNotificationEventConsumer> logger) : base(responseRestService, logger)
        {
            _notificationRestService = notificationRestService;
        }

        public async Task Consume(ConsumeContext<IActionNotificationEvent> context)
        {
            DateTime actionStartDate = DateTime.UtcNow;
            try
            {
                if (context.Message != null && context.Message as IActionNotificationEvent != null)
                {
                    IActionNotificationEvent action = context.Message;
                    _logger.LogDebug($"ResponseActionNotificationEventConsumer: ActionId: '{action.ActionId}'.");
                    _logger.LogDebug($"ResponseActionNotificationEventConsumer: User: '{action.User}'.");
                    _logger.LogDebug($"ResponseActionNotificationEventConsumer: Message: '{action.Message}'.");
                    _logger.LogDebug($"ResponseActionNotificationEventConsumer: IsSilent: '{action.IsSilent}'.");

                    NotificationModel result = await _notificationRestService.SendNotification(new NotificationCreationModel()
                    {
                        ResponseId = action.ResponseId,
                        NotificationText = action.Message,
                        Status = 1,
                        Title = "Alert Notification",
                        Tags = null
                    });

                    //Success
                    if (result != null)
                    {
                        await GenerateActionCallback(context, ActionStatus.Success, actionStartDate);
                        _logger.LogDebug($"ResponseActionNotificationEventConsumer: NotificationId: '{result.NotificationId}' published.");
                        return;
                    }

                    //Error
                    await GenerateActionCallback(context, ActionStatus.Error, actionStartDate, "The notification could not be sent.");
                    _logger.LogError("ResponseActionNotificationEventConsumer: The notification could not be sent.");
                }
                _logger.LogError("ResponseActionNotificationEventConsumer: Invalid Null or Empty Action Notification");
                throw new Exception("Invalid or Null Action Notification");
            }
            catch (Exception e)
            {
                await GenerateActionCallback(context, ActionStatus.Error, actionStartDate, $"The notification could not be sent: {e.Message}.");
                _logger.LogError($"ResponseActionNotificationEventConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
