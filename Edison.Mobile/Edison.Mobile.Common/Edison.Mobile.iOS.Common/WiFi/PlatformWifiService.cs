using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreFoundation;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.WiFi;
using Foundation;
using NetworkExtension;

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

        public async Task<WifiNetwork> GetCurrentlyConnectedWifiNetwork()
        {
            return await Task.FromResult(new WifiNetwork { SSID = "fakewifi" });
        }

        public async Task<bool> ConnectToSecuredWifiNetwork(string ssid, string passphrase)
        {
            try
            {
                var hotspotConfig = new NEHotspotConfiguration(ssid, passphrase, false);

                await NEHotspotConfigurationManager.SharedManager.ApplyConfigurationAsync(hotspotConfig);

                return true;
            }
            catch (Exception e)
            {
                logger.Log(e, "Failed to connect to hotspot");

                return false;
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
                    SSID = "EDISON_B827EB910E94",
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
