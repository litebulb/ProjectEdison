using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Api.Config;
using Edison.Api.Helpers;
using Edison.Common.Config;
using Edison.Common.Interfaces;
using Edison.Common.Messages;
using Edison.Core.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Edison.Api.Controllers
{
    [ApiController]
    [Route("api/Notifications")]
    public class NotificationHubController : Controller
    {
        private static IConfiguration _configuration;
        private static NotificationHubClient _hubClient;
        private readonly NotificationHubDataManager _notificationDataManager;
        private readonly DevicesDataManager _devicesDataManager;
        private static IOptions<NotificationsOptions> _notificationOptions;

        public NotificationHubController(IConfiguration configuration, IOptions<NotificationsOptions> notificationOptions, NotificationHubDataManager notificationDataManager, DevicesDataManager devicesDataManager)
        {
            _configuration = configuration;
            _notificationOptions = notificationOptions;
            _notificationDataManager = notificationDataManager;
            _devicesDataManager = devicesDataManager;
            _hubClient = NotificationHubClient
                                .CreateClientFromConnectionString(notificationOptions.Value.ConnectionString, notificationOptions.Value.PathName, true);
        }

        [Authorize(AuthenticationSchemes = "AzureAd,B2CWeb", Policy = "Consumer")]
        [Route("Register")]
        [Produces(typeof(DeviceMobileModel))]
        [HttpPost]
        public async Task<IActionResult> RegisterDevice([FromBody] DeviceRegistrationModel deviceRegistration)
        {
            try
            {
                RegistrationDescription registerDescription = null;
                DeviceMobileModel deviceModel = null;

                switch (deviceRegistration.Platform)
                {
                    case "apns":
                        registerDescription = deviceRegistration.Tags != null ? 
                            new AppleRegistrationDescription(deviceRegistration.Identifier, deviceRegistration.Tags) :
                            new AppleRegistrationDescription(deviceRegistration.Identifier);
                        break;
                    case "gcm":
                        registerDescription = deviceRegistration.Tags != null ?
                            new GcmRegistrationDescription(deviceRegistration.Identifier, deviceRegistration.Tags) :
                            new GcmRegistrationDescription(deviceRegistration.Identifier);
                        break;
                    default:
                        throw new ArgumentException("RegisterDevice: Unsupported platform registration");
                }
                try
                {
                    string newRegistrationId = null;

                    if (deviceRegistration.Identifier != null)
                    {
                        var registrations = await _hubClient.GetRegistrationsByChannelAsync(deviceRegistration.Identifier, 100);

                        foreach (RegistrationDescription registration in registrations)
                        {
                            if (newRegistrationId == null)
                            {
                                newRegistrationId = registration.RegistrationId;
                            }
                            else
                            {
                                await _hubClient.DeleteRegistrationAsync(registration);
                            }
                        }
                    }

                    if (newRegistrationId == null)
                        newRegistrationId = await _hubClient.CreateRegistrationIdAsync();
                    registerDescription.RegistrationId = newRegistrationId;
                    RegistrationDescription output = await _hubClient.CreateOrUpdateRegistrationAsync<RegistrationDescription>(registerDescription);
                    
                    if (output != null)
                         deviceModel = await _devicesDataManager.CreateOrUpdateDevice(
                            new DeviceMobileModel()
                            {
                                DeviceType = "Mobile",
                                Name = string.Concat("mobile_", deviceRegistration.Platform, "_", Guid.NewGuid()),
                                Email = deviceRegistration.UserEmail,
                                MobileId = deviceRegistration.Identifier,
                                Platform = deviceRegistration.Platform,
                                RegistrationId = output.RegistrationId
                            });
                    else
                        throw new Exception(string.Format("Registration creation or update failed. MobileId: {0}", deviceRegistration.Identifier));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return Ok(deviceModel);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status502BadGateway, e.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "AzureAd,B2CWeb", Policy = "Consumer")]
        [Route("Register")]
        [HttpDelete]
        [Produces(typeof(bool))]
        public async Task<IActionResult> DeleteDeviceRegistration(string registrationId)
        {
            try
            {
                await _hubClient.DeleteRegistrationAsync(registrationId);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            var deleteDevice = await _devicesDataManager.DeleteDevice(registrationId);
            return Ok(deleteDevice);
        }

        [Authorize(AuthenticationSchemes = "AzureAd,B2CWeb", Policy = "SuperAdmin")]
        [HttpPost]
        [Produces(typeof(List<NotificationOutcome>))]
        public async Task<IActionResult> SendNotification([FromBody] NotificationCreationModel notificationReq)
        {
            try
            {
                var jsonNotification = JsonConvert.SerializeObject(notificationReq);
                
                NotificationOutcome response = (notificationReq.Tags != null && notificationReq.Tags.Count > 0) ? 
                    await _hubClient.SendGcmNativeNotificationAsync("{\"data\":{\"message\":" + jsonNotification + "}}", notificationReq.Tags) :
                    await _hubClient.SendGcmNativeNotificationAsync("{\"data\":{\"message\":" + jsonNotification + "}}");

                NotificationOutcome responseApple = (notificationReq.Tags != null && notificationReq.Tags.Count > 0) ?
                    await _hubClient.SendAppleNativeNotificationAsync("{\"aps\":{\"alert\":\"" + notificationReq.Title + ": " + notificationReq.NotificationText + "\"}}", notificationReq.Tags) :
                    await _hubClient.SendAppleNativeNotificationAsync("{\"aps\":{\"alert\":\"" + notificationReq.Title + ": " + notificationReq.NotificationText + "\"}}");

                var outcomes = new List<NotificationOutcome>() { response, responseApple };
                NotificationModel model = null;

                if (response.Results != null && responseApple.Results != null)
                {
                    notificationReq.Status = 1;
                    model = await _notificationDataManager.CreateNotification(notificationReq);
                    return Ok(model);
                }
                else
                {
                    notificationReq.Status = 0;
                    model = await _notificationDataManager.CreateNotification(notificationReq);
                    return Ok(model);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status502BadGateway, e.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "AzureAd,B2CWeb", Policy = "Consumer")]
        [Route("Responses")]
        [HttpGet]
        [Produces(typeof(IEnumerable<NotificationModel>))]
        public async Task<IActionResult> GetNotificationsHistory([FromQuery]int pageSize, string continuationToken)
        {
            try
            {
                //logger.LogInformation("Get Notifications history list ");

                var notifications = await _notificationDataManager.GetNotifications(pageSize, continuationToken);
                return Ok(notifications);
            }
            catch (Exception e)
            {
                //logger.LogError(e, "Get Notifications history- {message}", e.Message);
                return StatusCode(StatusCodes.Status502BadGateway, e.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "AzureAd,B2CWeb", Policy = "Consumer")]
        [Route("Responses/{responseId}")]
        [HttpGet]
        [Produces(typeof(IEnumerable<NotificationModel>))]
        public async Task<IActionResult> GetNotificationsHistory(Guid responseId)
        {
            try
            {
                //logger.LogInformation("Get Notifications history list ");

                var notifications = await _notificationDataManager.GetNotifications(responseId);
                return Ok(notifications);
            }
            catch (Exception e)
            {
                //logger.LogError(e, "Get Notifications history- {message}", e.Message);
                return StatusCode(StatusCodes.Status502BadGateway, e.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "AzureAd,B2CWeb", Policy = "Admin")]
        [Route("Register")]
        [HttpGet]
        [Produces(typeof(CollectionQueryResult<RegistrationDescription>))]
        public async Task<IActionResult> GetRegisteredDevices([FromQuery]int pageSize, string continuationToken)
        {
            try
            {
                CollectionQueryResult<RegistrationDescription> devices = string.IsNullOrEmpty(continuationToken) ?
                    await _hubClient.GetAllRegistrationsAsync(pageSize) :
                    await _hubClient.GetAllRegistrationsAsync(continuationToken, pageSize);
                return Ok(devices);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status502BadGateway, e.Message);
            }
        }
    }
}
