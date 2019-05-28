using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed;
        public event EventHandler<CheckingConnectionStatusUpdatedEventArgs> CheckingConnectionStatusUpdated;

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

        public async Task<bool> ConnectToWifiNetwork(string ssid, string passphrase)
        {
            try
            {
                var hotspotConfig = string.IsNullOrEmpty(passphrase) ? new NEHotspotConfiguration(ssid) : new NEHotspotConfiguration(ssid, passphrase, false);
                hotspotConfig.JoinOnce = true;

                await NEHotspotConfigurationManager.SharedManager.ApplyConfigurationAsync(hotspotConfig);
                
                CheckingConnectionStatusUpdated?.Invoke(this, new CheckingConnectionStatusUpdatedEventArgs($"Connected", ssid, true));

                return true;
            }
            catch (Exception e)
            {
                ConnectionFailed?.Invoke(this, new ConnectionFailedEventArgs("Failed to connect to hotspot"));
                logger.Log(e, "Failed to connect to hotspot");

                return false;
            }
        }

        public async Task<bool> ConnectToWifiNetwork(string ssid)
        {
            return await ConnectToWifiNetwork(ssid, null);
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
