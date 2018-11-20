using Edison.Devices.Onboarding.Common.Helpers;
using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Helpers;
using Edison.Devices.Onboarding.Models.PortalAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Services
{
    internal class PortalService
    {
        public PortalService()
        {
        }

        public async Task<ResultCommandListFirmwares> GetFirmwares()
        {
            try
            {
                var allFirmwares = await LoadFirmwares();
                return new ResultCommandListFirmwares()
                {
                    Firmwares = allFirmwares.Select(p => p.PackageFullName),
                    IsSuccess = true
                };
            }
            catch(Exception e)
            {
                DebugHelper.LogError($"Error ListFirmwares: {e.Message}.");
                return ResultCommand.CreateFailedCommand<ResultCommandListFirmwares>($"Error ListFirmwares: {e.Message}.");
            }
        }

        public async Task<ResultCommand> SetDeviceSecretKeys(RequestCommandSetDeviceSecretKeys requestSetDeviceKeys)
        {
            try
            {
                //Set new Portal password
                if (!await PortalApiHelper.SetDevicePassword(SecretManager.PortalPassword, requestSetDeviceKeys.PortalPassword))
                {
                    return ResultCommand.CreateFailedCommand($"Error SetDevicePassword: Error while setting new device password.");
                }
                //Update Portal Password
                SecretManager.PortalPassword = requestSetDeviceKeys.PortalPassword;
                PortalApiHelper.Init("Administrator", SecretManager.PortalPassword);

                //Change Encryption Key
                SecretManager.EncryptionKey = requestSetDeviceKeys.EncryptionKey;

                //Set new Access Point - Will change the AP after reboot of the app
                //SecretManager.AccessPointSsid = SecretManager.AccessPointSsid;
                SecretManager.AccessPointPassword = requestSetDeviceKeys.AccessPointPassword;

                return ResultCommand.CreateSuccessCommand();
            }
            catch (Exception e)
            {
                DebugHelper.LogError($"Error SetDevicePassword: {e.Message}.");
                return ResultCommand.CreateFailedCommand($"Error SetDevicePassword: {e.Message}.");
            }
        }

        public async Task<ResultCommandSoftAPSettings> GetAccessPointSettings()
        {
            try
            {
                var apSettings = await PortalApiHelper.GetSoftAPSettings();
                return new ResultCommandSoftAPSettings()
                {
                    SoftAPSettings = apSettings,
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                DebugHelper.LogError($"Error GetAccessPointSettings: {e.Message}.");
                return ResultCommand.CreateFailedCommand<ResultCommandSoftAPSettings>($"Error GetAccessPointSettings: {e.Message}.");
            }
        }

        public async Task<ResultCommand> StopApp(string appName)
        {
            var apps = await PortalApiHelper.ListFirmwares();
            foreach(var app in apps.AppPackages)
            {
                if (app.PackageFullName.ToLower().StartsWith(appName.ToLower()))
                {
                    if (!await PortalApiHelper.StopHeadlessApp(app.PackageFullName))
                        return ResultCommand.CreateFailedCommand($"Error StopApp: Error while stopping App {app.PackageFullName}");
                    break;
                }
            }
            return ResultCommand.CreateSuccessCommand();
        }

        public async Task<ResultCommand> StartApp(string appName)
        {
            var apps = await PortalApiHelper.GetPackages();
            foreach (var app in apps.InstalledPackages)
            {
                if (app.PackageFullName.ToLower().StartsWith(appName.ToLower()))
                {
                    if (!await PortalApiHelper.StartHeadlessApp(app.PackageFullName))
                        return ResultCommand.CreateFailedCommand($"Error StopApp: Error while stopping App {app.PackageFullName}");
                    break;
                }
            }
            return ResultCommand.CreateSuccessCommand();
        }

        public async Task<ResultCommand> SetDeviceType(RequestCommandSetDeviceType requestDeviceType)
        {
            try
            {
                var apps = await LoadFirmwares();

                //Find the app
                var app = apps.FirstOrDefault(p => p.PackageFullName.ToLower().StartsWith(requestDeviceType.DeviceType.ToLower()));
                if (app == null)
                {
                    DebugHelper.LogError($"Package starting with {requestDeviceType.DeviceType} not found.");
                    return ResultCommand.CreateFailedCommand($"Error SetDeviceType: Package starting with {requestDeviceType.DeviceType} not found.");
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
                    return ResultCommand.CreateFailedCommand($"Error SetDeviceType: Error while starting App {app.PackageFullName}");
                }

                if (!await PortalApiHelper.SetStartupForHeadlessApp(true, app.PackageFullName))
                {
                    DebugHelper.LogError($"Error while turning on run at startup for App {app.PackageFullName}");
                    return ResultCommand.CreateFailedCommand($"Error SetDeviceType: Error while turning on run at startup for App {app.PackageFullName}");
                }

                return ResultCommand.CreateSuccessCommand();
            }
            catch(Exception e)
            {
                DebugHelper.LogError($"Error SetDeviceType: {e.Message}.");
                return ResultCommand.CreateFailedCommand($"Error SetDeviceType: {e.Message}.");
            }
        }

        public async Task<ResultCommand> SetDeviceName(RequestCommandSetDeviceName requestDeviceName)
        {
            try
            {
                if (!await PortalApiHelper.SetDeviceName(requestDeviceName.Name))
                {
                    DebugHelper.LogError($"Error while setting device name: {requestDeviceName.Name}");
                    return ResultCommand.CreateFailedCommand($"Error SetDeviceName: Error while setting device name: {requestDeviceName.Name}");
                }

                return ResultCommand.CreateSuccessCommand();
            }
            catch (Exception e)
            {
                DebugHelper.LogError($"Error SetDeviceName: {e.Message}.");
                return ResultCommand.CreateFailedCommand($"Error SetDeviceName: {e.Message}.");
            }
        }

        private async Task<IEnumerable<HeadlessApp>> LoadFirmwares()
        {
            var allFirmwares = await PortalApiHelper.ListFirmwares();
            return allFirmwares.AppPackages.Where(p =>
            p.PackageFullName.ToLower().StartsWith("edison.devices.") &&
            !p.PackageFullName.ToLower().StartsWith("edison.devices.onboarding"));
        }
    }
}
