using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;
using Edison.Core.Common;
using Edison.Core.Common.Models;
using Edison.Api.Config;
using Edison.Api.Helpers;

namespace Edison.Api.Controllers
{
    /// <summary>
    /// Controller to handle operations on notification
    /// </summary>
    [ApiController]
    [Route("api/Notifications")]
    public class NotificationHubController : Controller
    {
        private static IConfiguration _configuration;
        private static NotificationHubClient _hubClient;
        private readonly NotificationHubDataManager _notificationDataManager;
        private readonly DevicesDataManager _devicesDataManager;
        private static IOptions<NotificationsOptions> _notificationOptions;

        /// <summary>
        /// DI Constructor
        /// </summary>
        public NotificationHubController(IConfiguration configuration, IOptions<NotificationsOptions> notificationOptions, NotificationHubDataManager notificationDataManager, DevicesDataManager devicesDataManager)
        {
            _configuration = configuration;
            _notificationOptions = notificationOptions;
            _notificationDataManager = notificationDataManager;
            _devicesDataManager = devicesDataManager;
            _hubClient = NotificationHubClient
                                .CreateClientFromConnectionString(notificationOptions.Value.ConnectionString, notificationOptions.Value.PathName, true);
        }

        /// <summary>
        /// Register a device to a notification platform (android or ios) and add the device to
        /// the notification repository
        /// </summary>
        /// <param name="deviceRegistration">DeviceRegistrationModel</param>
        /// <returns>DeviceMobileModel</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Consumer)]
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

        /// <summary>
        /// Delete a registration from the notification repository
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns>True of the operation succeeded</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Consumer)]
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
            var deleteDevice = await _devicesDataManager.DeleteMobileDevice(registrationId);
            return Ok(deleteDevice);
        }

        /// <summary>
        /// Send a notification
        /// </summary>
        /// <param name="notificationReq">NotificationCreationModel</param>
        /// <returns>List of responses per platform</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.SuperAdmin)]
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

        /// <summary>
        /// Get notifications
        /// </summary>
        /// <param name="pageSize">Size of a result page</param>
        /// <param name="continuationToken">Continuation token</param>
        /// <returns>List of notifications</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Consumer)]
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

        /// <summary>
        /// Get notifications by Response Id
        /// </summary>
        /// <param name="responseId">Response Id</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Consumer)]
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

        /// <summary>
        /// Get all registered devices from the notification hub
        /// </summary>
        /// <param name="pageSize">Size of a result page</param>
        /// <param name="continuationToken">Continuation token</param>
        /// <returns>List of registrations</returns>
        [Authorize(AuthenticationSchemes = AuthenticationBearers.AzureADAndB2C, Policy = AuthenticationRoles.Admin)]
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
