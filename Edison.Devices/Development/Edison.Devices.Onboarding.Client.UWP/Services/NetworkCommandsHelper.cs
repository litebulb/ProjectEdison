using Edison.Devices.Onboarding.Client.Interfaces;
using Edison.Devices.Onboarding.Client.Models;
using Edison.Devices.Onboarding.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

namespace Edison.Devices.Onboarding.Client.UWP
{
    public class NetworkCommandsHelper : INetworkCommandsHelper
    {
        private readonly IAccessPointHelper _accessPoint;
        public event Action<string> ClientNetworkConnectedEvent;
        public event Action<string> ClientNetworksEnumeratedEvent;

        public NetworkCommandsHelper(IAccessPointHelper accessPointHelper)
        {
            _accessPoint = accessPointHelper;
        }

        public async Task RequestClientNetworks(ObservableCollection<Network> availableNetworks)
        {
            if (!_accessPoint.IsConnected) { return; }

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                availableNetworks.Clear();
            });

            var networkRequest = new Command() { BaseCommand = CommandsEnum.GetAvailableNetworks };
            // Send request for available networks
            await _accessPoint.SendRequest(networkRequest);

            // Read response with available networks                
            var networkResponse = await _accessPoint.GetNextRequest();
            if (networkResponse != null && networkResponse.BaseCommand == CommandsEnum.ResultGetAvailableNetworks)
            {
                ResultCommandAvailableNetworks dataAvailableNetworks = JsonConvert.DeserializeObject<ResultCommandAvailableNetworks>(networkResponse.Data);
                if (dataAvailableNetworks != null)
                {
                    dataAvailableNetworks.Networks.OrderBy(x => x).Distinct().ToList().ForEach(async availableNetwork =>
                    {
                        var network = new Network() { Ssid = availableNetwork };
                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            availableNetworks.Add(network);
                        });
                    });
                }
            }

            ClientNetworksEnumeratedEvent?.Invoke("Enumerated");
        }

        public async Task ConnectToClientNetwork(string networkSsid, string password)
        {
            if (!_accessPoint.IsConnected) { return; }

            string jsonNetworkInformation = JsonConvert.SerializeObject(new NetworkInformation() { Ssid = networkSsid, Password = password });
            var connectRequest = new Command() { BaseCommand = CommandsEnum.ConnectToNetwork, Data = jsonNetworkInformation };
            // Send request to connect to network
            await _accessPoint.SendRequest(connectRequest);

            // Read response with available networks
            var networkResponse = await _accessPoint.GetNextRequest();
            if (networkResponse != null && networkResponse.BaseCommand == CommandsEnum.ResultConnectToNetwork)
            {
                ResultCommandNetworkStatus dataNetworkStatus = JsonConvert.DeserializeObject<ResultCommandNetworkStatus>(networkResponse.Data);
                if(dataNetworkStatus != null)
                    ClientNetworkConnectedEvent?.Invoke(dataNetworkStatus.Status);
            }
        }

        public async Task DisconnectFromClientNetwork(string networkSsid)
        {
            if (!_accessPoint.IsConnected) { return; }

            var disconnectRequest = new Command() { BaseCommand = CommandsEnum.DisconnectFromNetwork, Data = networkSsid };
            // Send request to connect to network
            await _accessPoint.SendRequest(disconnectRequest);

            // Read response with available networks
            var networkResponse = await _accessPoint.GetNextRequest();
            if (networkResponse != null && networkResponse.BaseCommand == CommandsEnum.ResultDisconnectFromNetwork)
            {
                ResultCommandNetworkStatus dataNetworkStatus = JsonConvert.DeserializeObject<ResultCommandNetworkStatus>(networkResponse.Data);
                if (dataNetworkStatus != null)
                    ClientNetworkConnectedEvent?.Invoke(dataNetworkStatus.Status);
            }
        }
    }
}
