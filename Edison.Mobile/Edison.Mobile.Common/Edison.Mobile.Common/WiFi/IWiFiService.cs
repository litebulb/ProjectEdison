using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edison.Mobile.Common.WiFi
{
    public class ConnectionFailedEventArgs : EventArgs
    {
        public string FailureReason { get; private set; }
        public ConnectionFailedEventArgs(string failureReason)
        {
            this.FailureReason = failureReason;
        }
    }

    public class CheckingConnectionStatusUpdatedEventArgs : EventArgs
    {
        public string StatusText { get; private set; }
        public string SSID { get; private set; }

        public bool IsConnected { get; private set; }
        public CheckingConnectionStatusUpdatedEventArgs(string statusText, string ssid, bool isConnected)
        {
            this.StatusText = statusText;
            this.SSID = ssid;
            this.IsConnected = isConnected;
        }
    }
    public interface IWifiService
    {

        event EventHandler<ConnectionFailedEventArgs> ConnectionFailed;

        event EventHandler<CheckingConnectionStatusUpdatedEventArgs> CheckingConnectionStatusUpdated;

        Task<WifiNetwork> GetCurrentlyConnectedWifiNetwork();
        Task<bool> ConnectToWifiNetwork(string ssid);
        Task<bool> ConnectToWifiNetwork(string ssid, string passphrase);
        Task DisconnectFromWifiNetwork(WifiNetwork wifiNetwork);
    }
}
