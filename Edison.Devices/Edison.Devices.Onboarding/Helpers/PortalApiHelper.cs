using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Common.Models.CommandModels;
using Edison.Devices.Onboarding.Helpers;
using Edison.Devices.Onboarding.Models.PortalAPI;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Helpers
{
    internal class PortalApiHelper
    {
        private static RestClient _client;
        private static string _credentials;

        internal static void Init(string username, string password)
        {
            _client = new RestClient("http://localhost:8080/api/");
            _credentials = GetBase64String($"{username}:{password}");
        }

        private static string GetBase64String(string message)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(message));
        }

        private static RestRequest PrepareQuery(string endpoint, Method method)
        {
            RestRequest request = new RestRequest(endpoint, method);
            request.AddHeader("Authorization", $"Basic {_credentials}");
            return request;
        }

        internal static async Task<HeadlessModel> ListFirmwares()
        {
            RestRequest request = PrepareQuery("iot/appx/listHeadlessApps", Method.GET);
            var queryResult = await _client.ExecuteTaskAsync<HeadlessModel>(request);
            if (queryResult.IsSuccessful)
            {
                return queryResult.Data;
            }
            DebugHelper.LogCritical(queryResult.ErrorMessage);
            return null;
        }

        internal static async Task<PackageSetModel> GetPackages()
        {
            RestRequest request = PrepareQuery("app/packagemanager/packages", Method.GET);
            var queryResult = await _client.ExecuteTaskAsync<PackageSetModel>(request);
            if (queryResult.IsSuccessful)
            {
                return queryResult.Data;
            }
            DebugHelper.LogCritical(queryResult.ErrorMessage);
            return null;
        }

        internal static async Task<SoftAPSettings> GetSoftAPSettings()
        {
            RestRequest request = PrepareQuery("iot/iotonboarding/softapsettings", Method.GET);
            var queryResult = await _client.ExecuteTaskAsync<SoftAPSettings>(request);
            if (queryResult.IsSuccessful)
            {
                return queryResult.Data;
            }
            return null;
        }

        internal static async Task<bool> SetSoftAPSettings(bool enabled, string ssid, string password)
        {
            RestRequest request = PrepareQuery("iot/iotonboarding/softapsettings?SoftAPEnabled={SoftAPEnabled}&SoftApSsid={SoftApSsid}&SoftApPassword={SoftApPassword}", Method.POST);
            request.AddUrlSegment("SoftAPEnabled", enabled ? GetBase64String("true") : GetBase64String("false"));
            request.AddUrlSegment("SoftApSsid", GetBase64String(ssid));
            request.AddUrlSegment("SoftApPassword", GetBase64String(password));
            var queryResult = await _client.ExecuteTaskAsync<HttpStatusCode>(request);
            if (queryResult.IsSuccessful)
            {
                return true;
            }
            return false;
        }

        internal static async Task<bool> SetStartupForHeadlessApp(bool enabled, string package)
        {
            RestRequest request = PrepareQuery("iot/appx/startupHeadlessApp?appid={appid}", enabled ? Method.POST : Method.DELETE);
            request.AddUrlSegment("appid", GetBase64String(package));
            var queryResult = await _client.ExecuteTaskAsync<HttpStatusCode>(request);
            if (queryResult.IsSuccessful)
            {
                return true;
            }
            return false;
        }

        internal static async Task<bool> StartHeadlessApp(string package)
        {
            RestRequest request = PrepareQuery("iot/appx/app?appid={appid}", Method.POST);
            request.AddUrlSegment("appid", GetBase64String(package));
            var queryResult = await _client.ExecuteTaskAsync<HttpStatusCode>(request);
            if (queryResult.IsSuccessful)
            {
                return true;
            }
            return false;
        }

        internal static async Task<bool> StopHeadlessApp(string package)
        {
            var packageSet = await GetPackages();
            var packageObj = packageSet.InstalledPackages.Find(p => p.PackageRelativeId.ToLower() == package.ToLower());

            RestRequest request = PrepareQuery("taskmanager/app?package={package}", Method.DELETE);
            request.AddUrlSegment("package", GetBase64String(packageObj.PackageFullName));
            var queryResult = await _client.ExecuteTaskAsync<HttpStatusCode>(request);
            if (queryResult.IsSuccessful)
            {
                return true;
            }
            return false;
        }

        internal static async Task<bool> SetDevicePassword(string oldPassword, string newPassword)
        {
            RestRequest request = PrepareQuery("iot/device/password?oldpassword={oldpassword}&newpassword={newpassword}", Method.POST);
            request.AddUrlSegment("oldpassword", GetBase64String(oldPassword));
            request.AddUrlSegment("newpassword", GetBase64String(newPassword));
            var queryResult = await _client.ExecuteTaskAsync<HttpStatusCode>(request);
            if (queryResult.IsSuccessful)
            {
                return true;
            }
            return false;
        }
    }
}
