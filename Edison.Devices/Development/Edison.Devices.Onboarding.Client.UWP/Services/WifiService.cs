// Copyright (c) Microsoft. All rights reserved.

using Edison.Devices.Onboarding.Client.Interfaces;
using Edison.Devices.Onboarding.Client.Models;
using Edison.Devices.Onboarding.Common.Helpers;
using Edison.Devices.Onboarding.Common.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.WiFi;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Security.Credentials;
using Windows.Storage.Streams;
using Windows.System.Threading;

namespace Edison.Devices.Onboarding.Client.UWP
{
    public class WifiService : IAccessPointHelper
    {
        public event Action<string> AccessPointsEnumeratedEvent;
        public event Action<string> AccessPointConnectedEvent;
        public bool IsConnected { get { return _ConnectedSocket != null; } }

        private SemaphoreSlim _SocketLock = new SemaphoreSlim(1, 1);
        private StreamSocket _ConnectedSocket = null;
        private DataWriter _DataWriter = null;
        private DataReader _DataReader = null;
        private WiFiAdapter _connectedWifiAdapter = null;

        public async Task FindAccessPoints(ObservableCollection<AccessPoint> availableAccessPoints)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                availableAccessPoints.Clear();
            });

            // Add distinct AP Ssids in sorted order
            var wifiAdapterList = await WiFiAdapter.FindAllAdaptersAsync();
            wifiAdapterList.SelectMany(adapter => adapter.NetworkReport.AvailableNetworks).
                            Select(network => network.Ssid).
                            Distinct().
                            OrderBy(ssid => ssid).ToList().
                            ForEach(async ssid => {
                var ap = new AccessPoint() { Ssid = ssid };
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                    availableAccessPoints.Add(ap);
                });
            });

            AccessPointsEnumeratedEvent?.Invoke("Enumerated");
        }

        private async Task CreateStreamSocketClient(HostName hostName, string connectionPortString)
        {
            try
            {
                // Conect to port string that was sent
                var Client = new StreamSocket();
                Debug.WriteLine($"Opening socket: {hostName}:{connectionPortString}");
                await Client.ConnectAsync(hostName, connectionPortString, SocketProtectionLevel.PlainSocket);
                Debug.WriteLine($"Socket connected: {hostName}:{connectionPortString}");
                HandleSocket(Client);
            }
            catch (Exception)
            {
                // Handle failure here as desired.  In this sample, 
                // the failure will be handled by ConnectToAccessPoint
            }
        }

        public async Task DebugConnectToStreamServer()
        {
            await CreateStreamSocketClient(new HostName(SharedConstants.DEBUG_NETWORK_IP), SharedConstants.CONNECTION_PORT);
        }

        public async Task ConnectToAccessPoint(AccessPoint accessPoint)
        {
            var wifiAdapterList = await WiFiAdapter.FindAllAdaptersAsync();

            var wifiList = from adapter in wifiAdapterList from network in adapter.NetworkReport.AvailableNetworks select Tuple.Create(adapter, network);
            var apInfo = wifiList.Where(wifiInfo => wifiInfo.Item2.Ssid.Equals(accessPoint.Ssid)).First();

            WiFiConnectionResult result = null;
            if (apInfo != null)
            {
                var wifiNetwork = apInfo.Item2;
                var wiFiAdapter = apInfo.Item1;

                if (wifiNetwork.SecuritySettings.NetworkAuthenticationType == NetworkAuthenticationType.Open80211)
                {
                    Debug.WriteLine($"Opening connection to: {wifiNetwork.Ssid}");
                    result = await wiFiAdapter.ConnectAsync(wifiNetwork, WiFiReconnectionKind.Manual);
                }
                else
                {
                    PasswordCredential credential = new PasswordCredential
                    {
                        Password = "p@ssw0rd"
                    };

                    Debug.WriteLine($"Opening connection to using credentials: {wifiNetwork.Ssid} [{credential.Password}]");
                    result = await wiFiAdapter.ConnectAsync(wifiNetwork, WiFiReconnectionKind.Manual, credential);
                }

                if (result.ConnectionStatus == WiFiConnectionStatus.Success)
                {
                    Debug.WriteLine($"Connected successfully to: {wiFiAdapter.NetworkAdapter.NetworkAdapterId}.{wifiNetwork.Ssid}");
                    _connectedWifiAdapter = wiFiAdapter;

                    await CreateStreamSocketClient(new HostName(SharedConstants.SOFT_AP_IP), SharedConstants.CONNECTION_PORT);
                }
            }

            string connectionEventString = "Connected";
            if (_ConnectedSocket == null)
            {
                Debug.WriteLine($"Connection failed: {(result != null ? result.ConnectionStatus.ToString() : "access point not found")}");
                connectionEventString = "FailedConnected";
            }
            AccessPointConnectedEvent?.Invoke(connectionEventString);
        }

#pragma warning disable 1998
        public async Task Disconnect()
        {
            _SocketLock.Wait();

            try
            {
                if (_DataReader != null)
                {
                    _DataReader.Dispose();
                    _DataReader = null;
                }
                if (_DataWriter != null)
                {
                    _DataWriter.Dispose();
                    _DataWriter = null;
                }
                if (_ConnectedSocket != null)
                {
                    _ConnectedSocket.Dispose();
                    _ConnectedSocket = null;
                }
            }
            finally
            {
                _SocketLock.Release();
            }

            if (_connectedWifiAdapter != null)
            {
                var wifiAdapter = _connectedWifiAdapter;
                _connectedWifiAdapter = null;
                wifiAdapter.Disconnect();
            }

            Windows.UI.Xaml.Application.Current.Exit();
        }

        private void HandleSocket(StreamSocket socket)
        {
            _SocketLock.Wait();

            try
            {
                Debug.WriteLine($"Connection established from: {socket.Information.RemoteAddress.DisplayName}:{socket.Information.RemotePort}");
                _ConnectedSocket = socket;
                _DataWriter = new DataWriter(_ConnectedSocket.OutputStream);
                _DataReader = new DataReader(_ConnectedSocket.InputStream)
                {
                    InputStreamOptions = InputStreamOptions.Partial
                };
            }
            finally
            {
                _SocketLock.Release();
            }
        }

        public async Task SendRequest(Command communication)
        {
            await _SocketLock.WaitAsync();
            try
            {
                string requestData = CommandParserHelper.SerializeAndEncryptCommand(communication);
                _DataWriter.WriteString(requestData);
                await _DataWriter.StoreAsync();
                Debug.WriteLine($"Sent: {communication.BaseCommand}, {communication.Data}");
            }
            finally
            {
                _SocketLock.Release();
            }
        }

        public async Task<Command> GetNextRequest()
        {
            Command msg = null;

            await _SocketLock.WaitAsync();
            try
            {
                await _DataReader.LoadAsync(8192);

                string data = string.Empty;
                while (_DataReader.UnconsumedBufferLength > 0)
                {
                    data += _DataReader.ReadString(_DataReader.UnconsumedBufferLength);
                }

                if (data.Length != 0)
                {
                    msg = CommandParserHelper.DeserializeAndDecryptCommand(data);
                    Debug.WriteLine($"Incoming: {msg.BaseCommand}, {msg.Data}");
                }
            }
            finally
            {
                _SocketLock.Release();
            }

            return msg;
        }
    }
}
