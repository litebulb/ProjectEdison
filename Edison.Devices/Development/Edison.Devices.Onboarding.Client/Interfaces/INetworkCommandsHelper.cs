// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Edison.Devices.Onboarding.Client.Models;
using Edison.Devices.Onboarding.Common.Models;

namespace Edison.Devices.Onboarding.Client.Interfaces
{
    public interface INetworkCommandsHelper
    {
        event Action<string> ClientNetworkConnectedEvent;
        event Action<string> ClientNetworksEnumeratedEvent;

        Task RequestClientNetworks(ObservableCollection<Network> availableNetworks);
        Task ConnectToClientNetwork(string networkSsid, string password);
        Task DisconnectFromClientNetwork(string networkSsid);
    }
}
