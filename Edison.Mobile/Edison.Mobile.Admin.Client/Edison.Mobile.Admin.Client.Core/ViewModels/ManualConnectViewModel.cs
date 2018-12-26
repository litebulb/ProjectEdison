using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class ManualConnectViewModel : BaseViewModel
    {
        readonly DeviceSetupService deviceSetupService;
        readonly IWifiService wifiService;
        readonly OnboardingRestService onboardingRestService;

        public string DeviceTypeAsString => deviceSetupService.DeviceTypeAsString;

        public ObservableRangeCollection<WifiNetwork> AvailableWifiNetworks { get; } = new ObservableRangeCollection<WifiNetwork>();

        public ManualConnectViewModel(DeviceSetupService deviceSetupService, IWifiService wifiService, OnboardingRestService onboardingRestService)
        {
            this.deviceSetupService = deviceSetupService;
            this.wifiService = wifiService;
            this.onboardingRestService = onboardingRestService;
        }

        public override async void ViewAppeared()
        {
            base.ViewAppeared();

            var wifiNetworks = await wifiService.GetAvailableWifiNetworks();
            AvailableWifiNetworks.AddRange(wifiNetworks);
        }

        public async Task<bool> ConnectToDeviceHotspot(WifiNetwork wifiNetwork)
        {
            var success = await wifiService.ConnectToSecuredWifiNetwork(wifiNetwork.SSID, "Edison1234");

            deviceSetupService.CurrentDeviceHotspotNetwork = success ? wifiNetwork : null;

            await onboardingRestService.GetDeviceId();

            return success;
        }
    }
}
