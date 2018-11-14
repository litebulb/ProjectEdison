// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Edison.Devices.Onboarding.Client.Models;
using Edison.Devices.Onboarding.Common.Models;

namespace Edison.Devices.Onboarding.Client.Interfaces
{
    public interface IAccessPointHelper
    {
        event Action<string> AccessPointsEnumeratedEvent;
        event Action<string> AccessPointConnectedEvent;
        bool IsConnected { get; }

        Task FindAccessPoints(ObservableCollection<AccessPoint> availableAccessPoints);
        Task ConnectToAccessPoint(AccessPoint accessPoint);
        Task SendRequest(Command communication);
        Task<Command> GetNextRequest();
        Task Disconnect();
        Task DebugConnectToStreamServer();
    }
}
