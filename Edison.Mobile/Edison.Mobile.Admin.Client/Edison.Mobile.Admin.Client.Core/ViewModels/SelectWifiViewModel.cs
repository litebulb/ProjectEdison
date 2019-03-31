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

            this.wifiService.CheckingConnectionStatusUpdated += WifiService_CheckingConnectionStatusUpdated;
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
                await wifiService.ConnectToWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork.SSID);               
            }
            else
            {
                refreshAvailableNetworksTimer = new System.Timers.Timer(5000);

                refreshAvailableNetworksTimer.Elapsed += HandleRefreshTimerElapsed;
            }

        }

        private void WifiService_CheckingConnectionStatusUpdated(object sender, Common.WiFi.CheckingConnectionStatusUpdatedEventArgs e)
        {
            if (e.IsConnected && DeviceSetupService.SSIDIsEdisonDevice(deviceSetupService.CurrentDeviceHotspotNetwork.SSID))
            {
                refreshAvailableNetworksTimer = new System.Timers.Timer(5000);

                refreshAvailableNetworksTimer.Elapsed += HandleRefreshTimerElapsed;
                refreshAvailableNetworksTimer.Start();
            }
        }

        public override void ViewDisappeared()
        {
            base.ViewDisappeared();
            if (refreshAvailableNetworksTimer != null)
            {
                refreshAvailableNetworksTimer.Elapsed -= HandleRefreshTimerElapsed;
                refreshAvailableNetworksTimer.Stop();
            }
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
