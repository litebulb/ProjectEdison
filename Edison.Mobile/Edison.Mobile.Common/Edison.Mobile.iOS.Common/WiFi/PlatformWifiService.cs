using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Mobile.Common.WiFi;
using NetworkExtension;

namespace Edison.Mobile.iOS.Common.WiFi
{
    public class PlatformWifiService : IWifiService
    {
        public Task<bool> ConnectToSecuredNetwork(string ssid, string passphrase)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<WifiNetwork>> GetWifiNetworks() // mock
        {
            await Task.Delay(1000);

            return new WifiNetwork[]
            {
                new WifiNetwork
                {
                    SSID = "EDISON_B827EB910E94",
                },
            };
        }
    }
}
