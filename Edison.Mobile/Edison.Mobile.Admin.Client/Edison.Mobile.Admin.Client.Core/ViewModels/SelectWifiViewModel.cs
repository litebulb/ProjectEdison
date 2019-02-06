using System;
using System.Collections.Generic;
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

        public List<WifiNetwork> AvailableWifiNetworks { get; private set; }

        public event ViewNotification OnAvailableWifiNetworksChanged;

        public SelectWifiViewModel(OnboardingRestService onboardingRestService, DeviceSetupService deviceSetupService)
            : base(deviceSetupService)
        {
            this.onboardingRestService = onboardingRestService;
        }

        public override async void ViewAppeared()
        {
            base.ViewAppeared();
            await RefreshAvailableWifiNetworks();
        }

        public async Task RefreshAvailableWifiNetworks()
        {
            var networks = await onboardingRestService.GetAvailableWifiNetworks();
            if (networks != null)
            {
                var list = new List<WifiNetwork>(networks);
                if (list.Count == 0)
                {
                    list.Add(new WifiNetwork
                    {
                        SSID = "OffTheShortRail",
                    });
                }
                AvailableWifiNetworks = list;
                OnAvailableWifiNetworksChanged?.Invoke();
            }
        }
    }
}
