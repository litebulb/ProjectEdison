using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class ManualConnectViewModel : BaseViewModel
    {
        readonly DeviceSetupService deviceSetupService;

        public string DeviceTypeAsString => deviceSetupService.DeviceTypeAsString;

        public ObservableRangeCollection<WifiNetwork> AvailableWifiNetworks { get; } = new ObservableRangeCollection<WifiNetwork>();

        public ManualConnectViewModel(DeviceSetupService deviceSetupService)
        {
            this.deviceSetupService = deviceSetupService;
        }

        public override async void ViewAppeared()
        {
            base.ViewAppeared();

            var wifiNetworks = await deviceSetupService.GetAvailableWifiNetworks();
            AvailableWifiNetworks.AddRange(wifiNetworks);
        }

        public async Task<bool> ConnectToDeviceHotspot(WifiNetwork wifiNetwork) => await deviceSetupService.ConnectToDeviceHotspot(wifiNetwork);
    }
}
