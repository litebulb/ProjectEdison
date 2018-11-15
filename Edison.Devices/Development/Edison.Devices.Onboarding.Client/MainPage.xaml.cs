// Copyright (c) Microsoft. All rights reserved.

using Edison.Devices.Onboarding.Client.Interfaces;
using Edison.Devices.Onboarding.Client.Models;
using Edison.Devices.Onboarding.Client.Services;
using Edison.Devices.Onboarding.Common.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Xamarin.Forms;

namespace Edison.Devices.Onboarding.Client
{
    public partial class MainPage : ContentPage
    {
        enum State
        {
            Started,
            AccessPointsEnmerated,
            AccessPointSelected,
            AccessPointConnected,
            NetworksRequested,
            NetworksEnumerated,
            NetworkSelected,
            NetworkInfoSent,
            NetworkConnected,
        }

        private State CurrentState { get; set; }

        private void HandleState(State nextState)
        {
            _connectButton.IsEnabled = (nextState != State.Started) && (_availableAccessPointListView.SelectedItem != null);
            _requestClientNetworks.IsEnabled = (nextState >= State.AccessPointConnected);
            _clientNetworkPassword.IsEnabled = (nextState >= State.NetworksEnumerated) && (_availableNetworkListView.SelectedItem != null);
            _connectClientNetwork.IsEnabled = (nextState >= State.NetworksEnumerated) && (_availableNetworkListView.SelectedItem != null);

            _ScanApsGrouping.BorderColor = (nextState == State.Started) ? Color.Yellow : Color.Default;
            _ConnectApGrouping.BorderColor = (nextState == State.AccessPointsEnmerated || nextState == State.AccessPointSelected) ? Color.Yellow : Color.Default;
            _NetworksGrouping.BorderColor = (nextState == State.AccessPointConnected) ? Color.Yellow : Color.Default;
            _ConnectGrouping.BorderColor = (nextState == State.NetworksEnumerated || nextState == State.NetworkSelected) ? Color.Yellow : Color.Default;
            _DisconnectGrouping.BorderColor = (nextState == State.NetworkConnected) ? Color.Yellow : Color.Default;

            CurrentState = nextState;
        }

        public static IStreamSockerClient StreamSockerClient { get; set; }
        public static IAccessPointHelper AccessPointHelper { get; set; }
        public static ICommandsHelper CommandsHelper { get; set; }
        private ObservableCollection<AccessPoint> _AvailableAccessPoints = new ObservableCollection<AccessPoint>();
        private ObservableCollection<Network> _AvailableNetworks = new ObservableCollection<Network>();

        private bool AccessPointsScanned { get; set; }
        private bool AccessPointConnected { get; set; }
        private bool ClientNetworksEnumerated { get; set; }
        private bool ClientNetworkConnected { get; set; }

        public MainPage()
        {
            InitializeComponent();

            _connectButton.IsEnabled = false;

            _availableAccessPointListView.ItemsSource = _AvailableAccessPoints;
            _availableNetworkListView.ItemsSource = _AvailableNetworks;

            AccessPointHelper.AccessPointConnectedEvent += AccessPointHelper_AccessPointConnectedEvent;
            AccessPointHelper.AccessPointsEnumeratedEvent += AccessPointHelper_AccessPointsEnumeratedEvent;
            //NetworkCommandsHelper.ClientNetworkConnectedEvent += AccessPointHelper_ClientNetworkConnectedEvent;
            //NetworkCommandsHelper.ClientNetworksEnumeratedEvent += AccessPointHelper_ClientNetworksEnumeratedEvent;

            AccessPointsScanned = false;
            AccessPointConnected = false;
            ClientNetworkConnected = false;
            ClientNetworksEnumerated = false;

            _clientNetworkPassword.IsEnabled = false;
            _connectClientNetwork.IsEnabled = false;
            _connectButton.IsEnabled = false;
            _requestClientNetworks.IsEnabled = false;

            _ScanApsGrouping.BorderColor = Color.Yellow;
            _ConnectApGrouping.BorderColor = Color.Default;
            _NetworksGrouping.BorderColor = Color.Default;
            _DisconnectGrouping.BorderColor = Color.Default;
            _ConnectGrouping.BorderColor = Color.Default;
        }

        private void UpdateStatus(string action, string result)
        {
            _Status.Text = string.Format("{0} ... {1}", action, result);
        }

        private void UpdateStatusError(string action, string result)
        {
            _Status.Text = string.Format("{0} ... Error: {1}", action, result);
        }

        private void AccessPointHelper_ClientNetworksEnumeratedEvent(string status)
        {
            Device.BeginInvokeOnMainThread(() => {

                HandleState(State.NetworksEnumerated);
                UpdateStatus(null, status);
            });
        }

        private void AccessPointHelper_ClientNetworkConnectedEvent(string status)
        {
            Device.BeginInvokeOnMainThread(() => {
                HandleState(State.NetworkConnected);
                UpdateStatus(null, status);
            });
        }

        private void AccessPointHelper_AccessPointsEnumeratedEvent(string status)
        {
            Device.BeginInvokeOnMainThread(() => {
                HandleState(State.AccessPointsEnmerated);
                UpdateStatus(null, status);
            });
        }

        private void AccessPointHelper_AccessPointConnectedEvent(string status)
        {
            Device.BeginInvokeOnMainThread(() => {
                HandleState(State.AccessPointConnected);
                UpdateStatus(null, status);

#if AUTOMATE_FOR_TESTING
                AccessPointHelper.RequestClientNetworks(_AvailableNetworks);
#endif
            });
        }

        ~MainPage()
        {
            AccessPointHelper.Disconnect();
        }

        void SelectAccessPoint(object sender, SelectedItemChangedEventArgs e)
        {
            HandleState(State.AccessPointSelected);
        }

        void SelectClientNetwork(object sender, SelectedItemChangedEventArgs e)
        {
            HandleState(State.NetworkSelected);
        }

        void ConnectClientNetwork(object sender, System.EventArgs e)
        {
            UpdateStatus("Connecting to client network", "");
            var network = _availableNetworkListView.SelectedItem as Network;
            //var task = NetworkCommandsHelper.ConnectToClientNetwork(network.Ssid, _clientNetworkPassword.Text);
            //task.ContinueWith(t => { HandleException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        void DisconnectClientNetwork(object sender, System.EventArgs e)
        {
            UpdateStatus("Disconnecting from client network", "");
            var network = _availableNetworkListView.SelectedItem as Network;
            //var task = NetworkCommandsHelper.DisconnectFromClientNetwork(network.Ssid);
            //task.ContinueWith(t => { HandleException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        void ConnectToAccessPoint(object sender, System.EventArgs e)
        {
            UpdateStatus("Connecting to access point", "");
            var accessPoint = _availableAccessPointListView.SelectedItem as AccessPoint;
            //var task = AccessPointHelper.ConnectToAccessPoint(accessPoint);
            //task.ContinueWith(t => { HandleException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void RequestClientNetworks(object sender, System.EventArgs e)
        {
            UpdateStatus("Getting available client networks", "");
            //var task = NetworkCommandsHelper.RequestClientNetworks(_AvailableNetworks);
            //task.ContinueWith(t => { HandleException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void FindAccessPoints(object sender, System.EventArgs e)
        {
            UpdateStatus("Getting available access points", "");
            //var task = AccessPointHelper.FindAccessPoints(_AvailableAccessPoints);
        }

        public async void CommandGetDeviceId(object sender, EventArgs e)
        {
            UpdateStatus($"CommandGetDeviceId", "");
            var resultGetDevice = await CommandsHelper.GetDeviceId();
            if(resultGetDevice.IsSuccess)
                UpdateStatus($"CommandGetDeviceId", resultGetDevice.DeviceId);
            else
                UpdateStatusError($"CommandGetDeviceId", resultGetDevice.ErrorMessage);
        }

        public async void CommandListFirmwares(object sender, EventArgs e)
        {
            UpdateStatus($"CommandListFirmwares", "");
            var resultListFirmwares = await CommandsHelper.ListFirmwares();
            if (resultListFirmwares.IsSuccess)
                UpdateStatus($"CommandListFirmwares", string.Join(", ", resultListFirmwares.Firmwares));
            else
                UpdateStatusError($"CommandListFirmwares", resultListFirmwares.ErrorMessage);
        }

        public async void CommandGetAccessPointSettings(object sender, EventArgs e)
        {
            UpdateStatus($"CommandGetAccessPointSettings", "");
            var resultGetAvailableNetworks = await CommandsHelper.GetAccessPointSettings();
            if (resultGetAvailableNetworks.IsSuccess)
                UpdateStatus($"CommandGetAccessPointSettings", $"Enabled: {resultGetAvailableNetworks.SoftAPSettings.SoftAPEnabled}, SSID: {resultGetAvailableNetworks.SoftAPSettings.SoftApSsid}, Password: {resultGetAvailableNetworks.SoftAPSettings.SoftApPassword}");
            else
                UpdateStatusError($"CommandGetAccessPointSettings", resultGetAvailableNetworks.ErrorMessage);
        }

        public async void CommandSetDeviceSecretKeys(object sender, EventArgs e)
        {
            UpdateStatus($"CommandSetDeviceSecretKeys", "");
            var resultGetAvailableNetworks = await CommandsHelper.SetDeviceSecretKeys(new RequestCommandSetDeviceSecretKeys() //This will be retrieve from a REST endpoint
            {
                 PortalPassword = "Edison12345",
                 APSsid = "EDISON",
                 APPassword = "Edison12345",
                 SocketPassphrase = "DONOTRUN"
            });
            if (!resultGetAvailableNetworks.IsSuccess)
                UpdateStatusError($"CommandSetDeviceSecretKeys", resultGetAvailableNetworks.ErrorMessage);
            UpdateStatusError($"CommandSetDeviceSecretKeys", "Device secret keys reset.");
        }

        public async void CommandProvisionDevice(object sender, EventArgs e)
        {
            UpdateStatus($"CommandProvisionDevice", "");

            //Generate CSR
            var resultGenerateCSR = await CommandsHelper.GenerateCSR();
            if (!resultGenerateCSR.IsSuccess)
            {
                UpdateStatusError($"CommandProvisionDevice", resultGenerateCSR.ErrorMessage);
                return;
            }

            //Sign Certificate
            string token = "ADTOKEN"; //TODO Retrieve AzureAD token from current phone session
            DeviceProvisioningRestService client = new DeviceProvisioningRestService("https://edisonapidev.eastus.cloudapp.azure.com/deviceprovisioning/", token);
            var certificate = await client.GenerateCertificate(new DeviceCertificateRequestModel()
            {
                DeviceType = "Edison.Devices.SoundSensor", //TODO Need it dynamic from ListFirmwares
                Csr = resultGenerateCSR.Csr
            });
            if(certificate == null)
            {
                UpdateStatusError($"CommandProvisionDevice", "The signed certificate could not be retrieved.");
                return;
            }

            //Provision device
            var resultProvisionDevice = await CommandsHelper.ProvisionDevice(new RequestCommandProvisionDevice() { DeviceCertificateInformation = certificate });
            if (!resultProvisionDevice.IsSuccess)
            {
                UpdateStatusError($"CommandProvisionDevice", resultProvisionDevice.ErrorMessage);
                return;
            }

            //Set Device Type
            var resultSetDeviceType = await CommandsHelper.SetDeviceType(new RequestCommandSetDeviceType() { DeviceType = certificate.DeviceType });
            if (!resultSetDeviceType.IsSuccess)
            {
                UpdateStatusError($"CommandProvisionDevice", resultSetDeviceType.ErrorMessage);
                return;
            }

            UpdateStatus($"CommandProvisionDevice", "Provisionning complete");
        }
        
        public void Exit(object sender, EventArgs e)
        {
            AccessPointHelper.Disconnect();
        }
    }
}
