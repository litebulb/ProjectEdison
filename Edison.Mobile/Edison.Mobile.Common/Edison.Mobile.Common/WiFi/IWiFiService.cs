using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edison.Mobile.Common.WiFi
{
    public interface IWifiService
    {
        Task<IEnumerable<WifiNetwork>> GetPreviouslyConfiguredWifiNetworks();
        Task<WifiNetwork> GetCurrentlyConnectedWifiNetwork();
        Task<bool> ConnectToWifiNetwork(string ssid, string passphrase = null);
        Task<IEnumerable<WifiNetwork>> GetAvailableWifiNetworks();
        Task DisconnectFromWifiNetwork(WifiNetwork wifiNetwork);
    }
}
