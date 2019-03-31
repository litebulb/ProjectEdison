using System;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Core.Ioc;
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
        readonly ILogger logger;
        private string ssid;
        private string password;

        public event EventHandler<bool> PasswordSetCompleted;

        public EnterWifiPasswordViewModel(
            DeviceSetupService deviceSetupService, 
            IOnboardingRestService onboardingRestService,
            DeviceProvisioningRestService deviceProvisioningRestService,
            ILogger logger,
            IWifiService wifiService
        ) : base(deviceSetupService, deviceProvisioningRestService, onboardingRestService, wifiService)
        {
            
            this.onboardingRestService.SetBasicAuthentication(deviceSetupService.PortalPassword);

            this.logger = logger;
            this.wifiService.CheckingConnectionStatusUpdated += WifiService_CheckingConnectionStatusUpdated;

        }

        private async void WifiService_CheckingConnectionStatusUpdated(object sender, Common.WiFi.CheckingConnectionStatusUpdatedEventArgs e)
        {
            if(e.IsConnected && DeviceSetupService.SSIDIsEdisonDevice(e.SSID))
            {
                var result = await onboardingRestService.ConnectToNetwork(new RequestNetworkInformationModel
                {
                    NetworkInformation = new NetworkInformationModel
                    {
                        Ssid = ssid,
                        Password = password,
                    },
                });

                if (result.IsSuccess)
                {
                    //change wifi back
                    await wifiService.DisconnectFromWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork);                    
                }

                PasswordSetCompleted?.Invoke(this, result.IsSuccess);
            }
        }

        public async Task<bool> ConnectDeviceToNetwork(string ssid, string password)
        {
            this.ssid = ssid;
            this.password = password;
            try
            {
                var currentlyConnectedWifiNetwork = await wifiService.GetCurrentlyConnectedWifiNetwork();
                if (!DeviceSetupService.SSIDIsEdisonDevice(currentlyConnectedWifiNetwork.SSID))
                {
                    await wifiService.ConnectToWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork.SSID, deviceSetupService.WiFiPassword);
                }

                return true;
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
