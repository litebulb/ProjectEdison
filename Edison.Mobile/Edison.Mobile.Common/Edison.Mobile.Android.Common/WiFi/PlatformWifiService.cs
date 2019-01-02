using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Android.Common.WiFi
{
    public class PlatformWifiService : IWifiService
    {
        public Task<bool> ConnectToSecuredNetwork(string ssid, string passphrase)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WifiNetwork>> GetWifiNetworks()
        {
            throw new NotImplementedException();
        }

        Task<bool> IWifiService.ConnectToSecuredWifiNetwork(string ssid, string passphrase)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<WifiNetwork>> IWifiService.GetAvailableWifiNetworks()
        {
            throw new NotImplementedException();
        }

        Task<WifiNetwork> IWifiService.GetCurrentlyConnectedWifiNetwork()
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<WifiNetwork>> IWifiService.GetPreviouslyConfiguredWifiNetworks()
        {
            throw new NotImplementedException();
        }
    }
}
