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

                if(result.IsSuccess)
                {
                    //change wifi back
                    await wifiService.ConnectToWifiNetwork(deviceSetupService.OriginalSSID);
                }

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
