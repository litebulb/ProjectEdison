using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Helpers;
using Edison.Devices.Onboarding.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.Security.Credentials;

namespace Edison.Devices.Onboarding.Services
{
    internal class WifiService
    {
        public WifiService()
        {

        }

        public async Task<WifiSet> FindWifi(string ssid)
        {
            var wifiAdapterList = await WiFiAdapter.FindAllAdaptersAsync();
            var wifiList = from adapter in wifiAdapterList
                           from network in adapter.NetworkReport.AvailableNetworks
                           where network.Ssid.Equals(ssid)
                           select new WifiSet() { Adapter = adapter, Network = network };
            return wifiList.First();
        }

        public async Task<ResultCommandNetworkStatus> ConnectToNetworkHandler(NetworkInformation networkInfo)
        {
            var wifiSet = await FindWifi(networkInfo.Ssid);
            if (wifiSet != null)
            {
                PasswordCredential credential = new PasswordCredential
                {
                    Password = networkInfo.Password
                };

                DebugHelper.LogInformation($"Connecting to network using credentials: {wifiSet.Network.Ssid} [{credential.Password}]");
                var result = await wifiSet.Adapter.ConnectAsync(wifiSet.Network, WiFiReconnectionKind.Manual, credential);
                return new ResultCommandNetworkStatus() { Code = (int)result.ConnectionStatus, Status = result.ConnectionStatus == WiFiConnectionStatus.Success ? "Connected" : result.ConnectionStatus.ToString() };
            }
            return new ResultCommandNetworkStatus() { Code = 0, Status = "UnspecifiedFailure" };
        }

        public async Task<ResultCommandNetworkStatus> DisconnectFromNetworkHandler(string networkSsid)
        {
            var wifiSet = await FindWifi(networkSsid);
            if (wifiSet != null)
            {
                DebugHelper.LogInformation($"Disconnecting from network: {wifiSet.Network.Ssid}");
                wifiSet.Adapter.Disconnect();
                return new ResultCommandNetworkStatus() { Code = 1, Status = "Disconnected" };
            }
            return new ResultCommandNetworkStatus() { Code = 0, Status = "UnspecifiedFailure" };
        }

        public async Task<ResultCommandAvailableNetworks> GetAvailableNetworkListHandler()
        {
            var wifiAdapterList = await WiFiAdapter.FindAllAdaptersAsync();
            var wifiList = from adapter in wifiAdapterList
                           from network in adapter.NetworkReport.AvailableNetworks
                           select network.Ssid;
            var networks = wifiList.OrderBy(x => x).Distinct();

            return new ResultCommandAvailableNetworks() { Networks = networks };
        }
    }
}
