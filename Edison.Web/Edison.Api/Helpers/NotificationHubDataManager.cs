using Edison.Api.Config;
using Edison.Core.Common.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using System;
using Microsoft.Azure.NotificationHubs;

namespace Edison.Api.Helpers
{
    public class NotificationHubDataManager
    {
        private readonly WebApiConfiguration _config;
        //private readonly IMapper _mapper;
        private static string _notificationHubConnectionString;
        private static NotificationHubClient _hubClient;

        public NotificationHubDataManager(IOptions<WebApiConfiguration> config, string notificationHubConnectionString, string pathName)
        {
            _config = config.Value;
            _notificationHubConnectionString = notificationHubConnectionString;
            _hubClient = NotificationHubClient
                                .CreateClientFromConnectionString(notificationHubConnectionString, pathName, true);

        }


        public async Task<Boolean> RegisterMobileDevice(MobileDeviceNotificationHubInstallationModel deviceUpdate)
        {
            Dictionary<string, InstallationTemplate> templates = new Dictionary<string, InstallationTemplate>();
            foreach (var t in deviceUpdate.Templates)
            {
                templates.Add(t.Key, new InstallationTemplate { Body = t.Value.Body });
            }
            Installation installation = new Installation()
            {
                InstallationId = deviceUpdate.InstallationId,
                PushChannel = deviceUpdate.PushChannel,
                Tags = deviceUpdate.Tags,
                Templates = templates
            };
            switch (deviceUpdate.Platform)
            {
                case "apns":
                    installation.Platform = NotificationPlatform.Apns;
                    break;
                case "gcm":
                    installation.Platform = NotificationPlatform.Gcm;
                    break;
                default:
                    throw new ArgumentException("Bad Request");
            }
            installation.Tags = new List<string>(deviceUpdate.Tags);
            try
            {
                await _hubClient.CreateOrUpdateInstallationAsync(installation);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteDeviceInstallation(string id)
        {
            try
            {
                await _hubClient.DeleteInstallationAsync(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<NotificationOutcome> SendNotification(string payload, string tag)
        {
            try
            {
                var response = await _hubClient.SendGcmNativeNotificationAsync(payload, tag);
                return response;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task SendNotificationMultipleDevices(string payload, List<string> tags)
        {
            try
            {
                var splitTagsList = SplitList(tags, 20);
                // mag taglist size is 20
                foreach (var tagList in splitTagsList)
                {
                    var response = await _hubClient.SendGcmNativeNotificationAsync(payload, tagList);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static List<List<string>> SplitList(List<string> tags, int size)
        {
            var list = new List<List<string>>();
            for (int i = 0; i < tags.Count; i += size)
                list.Add(tags.GetRange(i, Math.Min(size, tags.Count - i)));
            return list;
        }
    }
}
