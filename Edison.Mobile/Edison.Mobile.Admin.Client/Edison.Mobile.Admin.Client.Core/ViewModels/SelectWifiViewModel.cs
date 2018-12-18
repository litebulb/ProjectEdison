using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class SelectWifiViewModel : BaseViewModel
    {
        readonly IWifiService wifiService;

        public List<WifiNetwork> AvailableWifiNetworks { get; private set; }

        public event EventHandler<List<WifiNetwork>> OnAvailableWifiNetworksChanged;

        public SelectWifiViewModel(IWifiService wifiService)
        {
            this.wifiService = wifiService;
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            Task.Run(RefreshAvailableWifiNetworks);
        }

        public async Task RefreshAvailableWifiNetworks()
        {
            var availableNetworks = await wifiService.GetAvailableWifiNetworks();

            AvailableWifiNetworks = new List<WifiNetwork>(availableNetworks);

            OnAvailableWifiNetworksChanged?.Invoke(this, AvailableWifiNetworks);
        }
    }
}
