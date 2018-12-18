using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edison.Mobile.Common.WiFi
{
    public interface IWifiService
    {
        Task<IEnumerable<WifiNetwork>> GetPreviouslyConfiguredWifiNetworks();
        Task<WifiNetwork> GetCurrentlyConnectedWifiNetwork();
        Task<bool> ConnectToSecuredWifiNetwork(string ssid, string passphrase);
        Task<IEnumerable<WifiNetwork>> GetAvailableWifiNetworks();
    }
}
