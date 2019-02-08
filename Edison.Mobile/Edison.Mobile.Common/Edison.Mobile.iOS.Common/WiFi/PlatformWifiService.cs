using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreFoundation;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.WiFi;
using Foundation;
using NetworkExtension;
using SystemConfiguration;

namespace Edison.Mobile.iOS.Common.WiFi
{
    public class PlatformWifiService : IWifiService
    {
        readonly ILogger logger;

        public PlatformWifiService(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task<WifiNetwork> GetCurrentlyConnectedWifiNetwork()
        {
            WifiNetwork wifiNetwork = null;

            CaptiveNetwork.TryCopyCurrentNetworkInfo("en0", out var info);

            if (info != null)
            {
                foreach (var pair in info)
                {
                    if (pair.Key.ToString() == "SSID")
                    {
                        wifiNetwork = new WifiNetwork
                        {
                            SSID = pair.Value.ToString(),
                        };
                    }
                }
            }

            return await Task.FromResult(wifiNetwork);
        }

        public async Task<bool> ConnectToWifiNetwork(string ssid, string passphrase = null)
        {
            try
            {
                var hotspotConfig = string.IsNullOrEmpty(passphrase) ? new NEHotspotConfiguration(ssid) : new NEHotspotConfiguration(ssid, passphrase, false);
                hotspotConfig.JoinOnce = true;

                await NEHotspotConfigurationManager.SharedManager.ApplyConfigurationAsync(hotspotConfig);

                return true;
            }
            catch (Exception e)
            {
                logger.Log(e, "Failed to connect to hotspot");

                return false;
            }
        }

        public async Task DisconnectFromWifiNetwork(WifiNetwork wifiNetwork)
        {
            if (wifiNetwork != null && wifiNetwork.SSID != null)
            {
                await Task.Run(() => NEHotspotConfigurationManager.SharedManager.RemoveConfiguration(wifiNetwork.SSID));
            }
        }
    }
}
