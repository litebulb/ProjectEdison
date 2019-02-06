using System;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Core.Models;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class EnterWifiPasswordViewModel : DeviceSetupBaseViewModel
    {
        readonly OnboardingRestService onboardingRestService;
        readonly IWifiService wifiService;
        readonly ILogger logger;

        public EnterWifiPasswordViewModel(
            DeviceSetupService deviceSetupService, 
            OnboardingRestService onboardingRestService,
            ILogger logger,
            IWifiService wifiService
        ) : base(deviceSetupService)
        {
            this.onboardingRestService = onboardingRestService;
            this.logger = logger;
            this.wifiService = wifiService;
        }

        public async Task<bool> ConnectDeviceToNetwork(string ssid, string password)
        {
            try
            {
                var result = await onboardingRestService.ConnectToNetwork(new RequestNetworkInformationModel
                {
                    NetworkInformation = new NetworkInformationModel
                    {
                        Ssid = ssid,
                        Password = password,
                    },
                });

                return result.IsSuccess;
            } 
            catch (Exception e)
            {
                logger.Log(e);
                return false;
            }
        }

        public async Task DisconnectFromDevice()
        {
            await wifiService.DisconnectFromWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork);
            await Task.Delay(2000);
        }
    }
}
