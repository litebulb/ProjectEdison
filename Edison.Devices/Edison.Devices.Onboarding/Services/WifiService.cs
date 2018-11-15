using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Helpers;
using Edison.Devices.Onboarding.Models;
using System;
using System.Linq;
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

        public async Task<ResultCommandNetworkStatus> ConnectToNetworkHandler(RequestCommandConnectToNetwork requestConnectToNetwork)
        {
            try
            {
                NetworkInformation networkInfo = requestConnectToNetwork.NetworkInformation;

                var wifiSet = await FindWifi(networkInfo.Ssid);
                if (wifiSet != null)
                {
                    PasswordCredential credential = new PasswordCredential
                    {
                        Password = networkInfo.Password
                    };

                    DebugHelper.LogInformation($"Connecting to network using credentials: {wifiSet.Network.Ssid} [{credential.Password}]");
                    var result = await wifiSet.Adapter.ConnectAsync(wifiSet.Network, WiFiReconnectionKind.Manual, credential);
                    return new ResultCommandNetworkStatus() { Code = (int)result.ConnectionStatus, IsSuccess = true, Status = result.ConnectionStatus == WiFiConnectionStatus.Success ? "Connected" : result.ConnectionStatus.ToString() };
                }
                return ResultCommand.CreateFailedCommand<ResultCommandNetworkStatus>($"Error ConnectToNetworkHandler: UnspecifiedFailure.");
            }
            catch(Exception e)
            {
                DebugHelper.LogError($"Error ConnectToNetworkHandler: {e.Message}.");
                return ResultCommand.CreateFailedCommand<ResultCommandNetworkStatus>($"Error ConnectToNetworkHandler: {e.Message}.");
            }
        }

        public async Task<ResultCommandNetworkStatus> DisconnectFromNetworkHandler(RequestCommandDisconnectFromNetwork requestDisconnectFromNetwork)
        {
            try
            {
                var wifiSet = await FindWifi(requestDisconnectFromNetwork.Ssid);
                if (wifiSet != null)
                {
                    DebugHelper.LogInformation($"Disconnecting from network: {wifiSet.Network.Ssid}");
                    wifiSet.Adapter.Disconnect();
                    return new ResultCommandNetworkStatus() { Code = 1, Status = "Disconnected", IsSuccess = true };
                }
                return ResultCommand.CreateFailedCommand<ResultCommandNetworkStatus>($"Error DisconnectFromNetworkHandler: UnspecifiedFailure.");
            }
            catch (Exception e)
            {
                DebugHelper.LogError($"Error DisconnectFromNetworkHandler: {e.Message}.");
                return ResultCommand.CreateFailedCommand<ResultCommandNetworkStatus>($"Error DisconnectFromNetworkHandler: {e.Message}.");
            }
        }

        public async Task<ResultCommandAvailableNetworks> GetAvailableNetworkListHandler()
        {
            try
            {
                var wifiAdapterList = await WiFiAdapter.FindAllAdaptersAsync();
                var wifiList = from adapter in wifiAdapterList
                               from network in adapter.NetworkReport.AvailableNetworks
                               select network.Ssid;
                var networks = wifiList.OrderBy(x => x).Distinct();

                return new ResultCommandAvailableNetworks() { Networks = networks, IsSuccess = true };
            }
            catch (Exception e)
            {
                DebugHelper.LogError($"Error GetAvailableNetworkListHandler: {e.Message}.");
                return ResultCommand.CreateFailedCommand<ResultCommandAvailableNetworks>($"Error GetAvailableNetworkListHandler: {e.Message}.");
            }
        }

        private async Task<WifiSet> FindWifi(string ssid)
        {
            var wifiAdapterList = await WiFiAdapter.FindAllAdaptersAsync();
            var wifiList = from adapter in wifiAdapterList
                           from network in adapter.NetworkReport.AvailableNetworks
                           where network.Ssid.Equals(ssid)
                           select new WifiSet() { Adapter = adapter, Network = network };
            return wifiList.First();
        }
    }
}
