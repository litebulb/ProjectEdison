using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Android.Common.WiFi
{
    public class PlatformWifiService : IWifiService
    {
        public Task<bool> ConnectToWifiNetwork(string ssid, string passphrase = null)
        {
            throw new NotImplementedException();
        }

        public Task DisconnectFromWifiNetwork(WifiNetwork wifiNetwork)
        {
            throw new NotImplementedException();
        }

        public Task<WifiNetwork> GetCurrentlyConnectedWifiNetwork()
        {
            throw new NotImplementedException();
        }
    }
}
