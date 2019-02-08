using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Common.Shared;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class SelectWifiViewModel : DeviceSetupBaseViewModel
    {
        readonly OnboardingRestService onboardingRestService;
        readonly IWifiService wifiService;
        readonly System.Timers.Timer refreshAvailableNetworksTimer;

        public List<WifiNetwork> AvailableWifiNetworks { get; private set; }

        public event ViewNotification OnAvailableWifiNetworksChanged;

        public SelectWifiViewModel(
            OnboardingRestService onboardingRestService, 
            DeviceSetupService deviceSetupService,
            IWifiService wifiService
        )
            : base(deviceSetupService)
        {
            this.onboardingRestService = onboardingRestService;
            this.wifiService = wifiService;

            refreshAvailableNetworksTimer = new System.Timers.Timer(3000);
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

            if (currentlyConnectedWifiNetwork == null || !DeviceSetupService.SSIDIsEdisonDevice(currentlyConnectedWifiNetwork.SSID))
            {
                if (deviceSetupService.CurrentDeviceHotspotNetwork != null)
                {
                    await wifiService.ConnectToWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork.SSID, deviceSetupService.DefaultPassword);
                    await Task.Delay(1000);
                }
                else
                {
                    // not connected to device, and don't have device wifi... need to go back and rescan or enter manually...
                    // TODO: lost connection with device... please go back to step 1
                }
            }

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

        public async Task RefreshAvailableWifiNetworks()
        {
            var networks = await onboardingRestService.GetAvailableWifiNetworks();
            if (networks != null)
            {
                var list = new List<WifiNetwork>(networks.Where(n => !string.IsNullOrWhiteSpace(n.SSID)));
                AvailableWifiNetworks = list;
                OnAvailableWifiNetworksChanged?.Invoke();
            }
        }
    }
}
