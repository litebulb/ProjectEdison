using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Common.Models.CommandModels;
using Edison.Devices.Onboarding.Helpers;
using Edison.Devices.Onboarding.Models;
using Edison.Devices.Onboarding.Models.PortalAPI;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.WiFi;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Edison.Devices.Onboarding.Services
{
    internal class PortalService
    {
        public PortalService()
        {
        }

        public async Task<IEnumerable<HeadlessApp>> GetFirmwares()
        {
            var allFirmwares = await PortalApiHelper.ListFirmwares();
            return allFirmwares.AppPackages.Where(p =>
            p.PackageFullName.ToLower().StartsWith("edison.devices.") &&
            !p.PackageFullName.ToLower().StartsWith("edison.devices.onboarding"));
        }

        public async Task<ResultCommandListFirmwares> ListFirmwares()
        {
            var allFirmwares = await GetFirmwares();
            return new ResultCommandListFirmwares()
            {
                Firmwares = allFirmwares.Select(p => p.PackageFullName)
            };
        }

        public async Task<ResultCommand> SetAccessPoint(SoftAPSettings softAPSettings)
        {
            if(!await PortalApiHelper.SetSoftAPSettings(softAPSettings.SoftAPEnabled, softAPSettings.SoftApSsid, softAPSettings.SoftApPassword))
            {
                return ResultCommand.CreateFailedCommand($"Error while setting access point.");
            }
            return ResultCommand.CreateSuccessCommand();
        }

        public async Task<ResultCommand> SetDevicePassword(ChangeDevicePassword changeDevicePassword)
        {
            if (!await PortalApiHelper.SetDevicePassword(changeDevicePassword.OldPassword, changeDevicePassword.NewPassword))
            {
                return ResultCommand.CreateFailedCommand($"Error while setting new device password.");
            }
            return ResultCommand.CreateSuccessCommand();
        }

        public async Task<ResultCommandSoftAPSettings> GetAccessPointSettings()
        {
            var apSettings = await PortalApiHelper.GetSoftAPSettings();
            return new ResultCommandSoftAPSettings()
            {
                SoftAPSettings = apSettings
            };
        }

        public async Task<ResultCommand> SetDeviceType(string deviceType)
        {
            var apps = await GetFirmwares();

            //Find the app
            var app = apps.FirstOrDefault(p => p.PackageFullName.ToLower().StartsWith(deviceType.ToLower()));
            if (app == null)
            {
                DebugHelper.LogError($"Package starting with {deviceType} not found.");
                return ResultCommand.CreateFailedCommand($"Package starting with {deviceType} not found.");
            }

            //Deactivate all other firmwares at startup
            foreach (var appToStop in apps)
            {
                if (appToStop.IsStartup)
                    if (!await PortalApiHelper.SetStartupForHeadlessApp(false, appToStop.PackageFullName))
                        DebugHelper.LogError($"Error while turning off run at startup for App {appToStop.PackageFullName}");
                if (!await PortalApiHelper.StopHeadlessApp(appToStop.PackageFullName))
                    DebugHelper.LogError($"Error while stopping App {appToStop.PackageFullName}");
            }

            //Start our firmware and set to run
            if (!await PortalApiHelper.StartHeadlessApp(app.PackageFullName))
            {
                DebugHelper.LogError($"Error while starting App {app.PackageFullName}");
                return ResultCommand.CreateFailedCommand($"Error while starting App {app.PackageFullName}");
            }

            if (!await PortalApiHelper.SetStartupForHeadlessApp(true, app.PackageFullName))
            {
                DebugHelper.LogError($"Error while turning on run at startup for App {app.PackageFullName}");
                return ResultCommand.CreateFailedCommand($"Error while turning on run at startup for App {app.PackageFullName}");
            }

            return ResultCommand.CreateSuccessCommand();
        }
    }
}
