// Copyright (c) Microsoft. All rights reserved.

using Edison.Devices.Onboarding.Client.Interfaces;
using Edison.Devices.Onboarding.Client.Models;
using Edison.Devices.Onboarding.Common.Helpers;
using Edison.Devices.Onboarding.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private WiFiAdapter _connectedWifiAdapter = null;

        public async Task<IEnumerable<AccessPoint>> FindAccessPoints()
        {
            List<AccessPoint> availableAccessPoints = new List<AccessPoint>();

            // Add distinct AP Ssids in sorted order
            var wifiAdapterList = await WiFiAdapter.FindAllAdaptersAsync();
            wifiAdapterList.SelectMany(adapter => adapter.NetworkReport.AvailableNetworks).
                            Select(network => network.Ssid).
                            Distinct().
                            OrderBy(ssid => ssid).ToList().
                            ForEach(ssid => {
                                var ap = new AccessPoint() { Ssid = ssid };
                                availableAccessPoints.Add(ap);
                            });

            return availableAccessPoints;
        }

        public async Task<WifiConnectionStatus> ConnectToAccessPoint(AccessPoint accessPoint)
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
                        Password = "yPGodsCrgy"// "Edison1234" //Need to be dynamic
                    };

                    Debug.WriteLine($"Opening connection to using credentials: {wifiNetwork.Ssid} [{credential.Password}]");
                    result = await wiFiAdapter.ConnectAsync(wifiNetwork, WiFiReconnectionKind.Manual, credential);
                }

                if (result.ConnectionStatus == WiFiConnectionStatus.Success)
                {
                    Debug.WriteLine($"Connected successfully to: {wiFiAdapter.NetworkAdapter.NetworkAdapterId}.{wifiNetwork.Ssid}");
                    _connectedWifiAdapter = wiFiAdapter;
                    return WifiConnectionStatus.Connected;
                }
                else
                {
                    Debug.WriteLine($"Connection failed: {(result != null ? result.ConnectionStatus.ToString() : "access point not found")}");
                }
            }
            return WifiConnectionStatus.FailedConnected;
        }

        public void Disconnect()
        {
            if (_connectedWifiAdapter != null)
            {
                var wifiAdapter = _connectedWifiAdapter;
                _connectedWifiAdapter = null;
                wifiAdapter.Disconnect();
            }
        }
    }
}
