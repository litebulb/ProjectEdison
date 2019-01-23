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
           var result = NEHotspotHelper.Register(new NEHotspotHelperOptions
            {
                DisplayName = new NSString("Pick a Wifi Network"),
            }, DispatchQueue.MainQueue, HandleNEHotspotHelperHandler);

            Console.WriteLine(result);
        }

        Task<WifiNetwork> IWifiService.GetCurrentlyConnectedWifiNetwork()
        {
            WifiNetwork wifiNetwork = null;

            CaptiveNetwork.TryCopyCurrentNetworkInfo("en0", out var info);

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

            return Task.FromResult(wifiNetwork);

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

        // iOS is unable to gather available wifi networks without apple's approval (https://developer.apple.com/documentation/networkextension/nehotspothelper).
        public async Task<IEnumerable<WifiNetwork>> GetAvailableWifiNetworks() // mock
        {
            await Task.Delay(1000);

            return new WifiNetwork[]
            {
                new WifiNetwork
                {

                },
            };
        }

        public async Task<IEnumerable<WifiNetwork>> GetPreviouslyConfiguredWifiNetworks()
        {
            try
            {
                var configuredSSIDs = await NEHotspotConfigurationManager.SharedManager.GetConfiguredSsidsAsync();
                return configuredSSIDs.Select(ssid => new WifiNetwork { SSID = ssid });
            }
            catch (Exception e)
            {
                logger.Log(e, "Failed to get configured SSIDs");

                return new WifiNetwork[] { };
            }
        }

        // handler for hotspot scan. won't work without proper entitlement, permission must be granted from apple
        void HandleNEHotspotHelperHandler(NEHotspotHelperCommand cmd)
        {
            switch (cmd.CommandType)
            {
                case NEHotspotHelperCommandType.FilterScanList:
                case NEHotspotHelperCommandType.Evaluate:
                    foreach (var network in cmd.NetworkList)
                    {
                        Console.WriteLine(network);
                    }
                    break;
            }
        }
    }
}
