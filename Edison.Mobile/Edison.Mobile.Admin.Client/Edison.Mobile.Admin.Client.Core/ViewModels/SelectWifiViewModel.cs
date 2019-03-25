using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.Models;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Common.Shared;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class SelectWifiViewModel : DeviceSetupBaseViewModel
    {
        private System.Timers.Timer refreshAvailableNetworksTimer;

        public List<AvailableNetwork> AvailableWifiNetworks { get; private set; }

        public event ViewNotification OnAvailableWifiNetworksChanged;


        public SelectWifiViewModel(
            IOnboardingRestService onboardingRestService, 
            DeviceSetupService deviceSetupService,
            IWifiService wifiService,
            DeviceProvisioningRestService deviceProvisioningRestService
        )
            : base(deviceSetupService, deviceProvisioningRestService, onboardingRestService, wifiService)
        {
            this.onboardingRestService.SetBasicAuthentication(deviceSetupService.PortalPassword);
        }

        public override async void ViewAppeared()
        {
            base.ViewAppeared();
            await RefreshAvailableWifiNetworks();
        }

        public override async void ViewAppearing()
        {
            base.ViewAppearing();

            var currentlyConnectedWifiNetwork = await wifiService.GetCurrentlyConnectedWifiNetwork();
                        
            if (!DeviceSetupService.SSIDIsEdisonDevice(currentlyConnectedWifiNetwork.SSID))
            {
                deviceSetupService.OriginalSSID = currentlyConnectedWifiNetwork.SSID;
                await wifiService.DisconnectFromWifiNetwork(currentlyConnectedWifiNetwork);

                var connected = await wifiService.ConnectToWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork.SSID);
                await wifiService.DisconnectFromWifiNetwork(new WifiNetwork() { SSID = deviceSetupService.CurrentDeviceHotspotNetwork.SSID } );
                if (!connected && deviceSetupService.CurrentDeviceHotspotNetwork != null)
                {
                    await wifiService.DisconnectFromWifiNetwork(currentlyConnectedWifiNetwork);
                    connected = await wifiService.ConnectToWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork.SSID, deviceSetupService.WiFiPassword);                    
                }
                else
                {
                    // not connected to device, and don't have device wifi... need to go back and rescan or enter manually...
                    // TODO: lost connection with device... please go back to step 1
                }
            }

            refreshAvailableNetworksTimer = new System.Timers.Timer(5000);

            refreshAvailableNetworksTimer.Elapsed += HandleRefreshTimerElapsed;
            refreshAvailableNetworksTimer.Start();
        }

        public override void ViewDisappeared()
        {
            base.ViewDisappeared();
            refreshAvailableNetworksTimer.Elapsed -= HandleRefreshTimerElapsed;
            refreshAvailableNetworksTimer.Stop();
        }

        async void HandleRefreshTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            await RefreshAvailableWifiNetworks();
        }

        public void SetWifi(string selectedSSid)
        {
            deviceSetupService.ConnectedWifiSSID = selectedSSid; 
        }

        public async Task RefreshAvailableWifiNetworks()
        {
            var networks = await onboardingRestService.GetAvailableWifiNetworks();
            if (networks != null)
            {
                var list = new List<AvailableNetwork>(networks.Where(n => !string.IsNullOrWhiteSpace(n.SSID) && !n.AlreadyConnected));
                AvailableWifiNetworks = list;
                OnAvailableWifiNetworksChanged?.Invoke();
            }
        }
    }
}
