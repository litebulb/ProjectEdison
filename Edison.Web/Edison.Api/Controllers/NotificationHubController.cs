using System;
using System.Threading.Tasks;
using Edison.Api.Helpers;
using Edison.Common.Interfaces;
using Edison.Common.Messages;
using Edison.Core.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Edison.Api.Controllers
{
    [Authorize(AuthenticationSchemes = "Backend,B2CWeb")]
    [Route("api/Notifications")]
    public class NotificationHubController : Controller
    {
        private readonly NotificationHubDataManager _notificationHubRepo;
        private readonly ICosmosDBRepository<NotificationModel> _cosmosDBRepository;

        public NotificationHubController(NotificationHubDataManager notificationHubRepo,
            ICosmosDBRepository<NotificationModel> cosmosDBRepository)
        {
            _notificationHubRepo = notificationHubRepo;
            _cosmosDBRepository = cosmosDBRepository;
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> RegisterDevice([FromBody] MobileDeviceNotificationHubInstallationEvent deviceInstallation)
        {
            try
            {
                var device = new MobileDeviceNotificationHubInstallationModel()
                {
                    InstallationId = deviceInstallation.InstallationId,
                    Platform = deviceInstallation.Platform,
                    PushChannel = deviceInstallation.PushChannel,
                    Tags = deviceInstallation.Tags,
                    Templates = deviceInstallation.Templates
                };

                //logger.LogInformation("Register Device in Notification Hub id- {0}", deviceInstallation.InstallationId);
                await _notificationHubRepo.RegisterMobileDevice(device);
                return Ok();
            }
            catch (Exception e)
            {
                //logger.LogError(e, "Register Device in Notification Hub- Exception {message}", e.Message);
                return StatusCode(StatusCodes.Status502BadGateway, e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] NotificationEvent notificationReq)
        {
            try
            {
                var jsonNotification = JsonConvert.SerializeObject(notificationReq);
                var response = await _notificationHubRepo.SendNotification("{\"data\":{\"message\":" + jsonNotification + "}}",
                    notificationReq.Notification.User);
                var notificationDoc = new NotificationModel()
                {
                    User = notificationReq.Notification.User,
                    CreationDate = notificationReq.Notification.CreationDate,
                    NotificationId = notificationReq.Notification.NotificationId,
                    NotificationText = notificationReq.Notification.NotificationText,
                    Status = notificationReq.Notification.Status,
                    Tags = notificationReq.Notification.Tags,
                    Title = notificationReq.Notification.Title,
                    UpdateDate = notificationReq.Notification.UpdateDate
                };
                if (response.Results != null)
                {
                    //foreach (RegistrationResult result in response.Results)
                    //{
                    //    logger.LogInformation("Notification log result Platform- {0}, registrationId- {1}, outcome- {2}",
                    //        result.ApplicationPlatform, result.RegistrationId, result.Outcome);

                    //}
                    notificationDoc.Status = 1;
                    await _cosmosDBRepository.CreateItemAsync(notificationDoc);
                    return Ok(response);
                }
                else
                {
                    //logger.LogError("Send notification failed to- {0}", notificationReq.User);
                    notificationDoc.Status = 0;
                    await _cosmosDBRepository.CreateItemAsync(notificationDoc);
                    return Ok(response);
                }
            }
            catch (Exception e)
            {
                //logger.LogError("Exception sending notification to- {0}", notificationReq.User);
                return StatusCode(StatusCodes.Status502BadGateway, e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationsHistory([FromQuery]int pageSize, string continuationToken)
        {
            try
            {
                //logger.LogInformation("Get Notifications history list ");

                var notifications = await _cosmosDBRepository.GetItemsPagingAsync(pageSize, continuationToken);
                return Ok(notifications);
            }
            catch (Exception e)
            {
                //logger.LogError(e, "Get Notifications history- {message}", e.Message);
                return StatusCode(StatusCodes.Status502BadGateway, e.Message);
            }
        }
    }
}
