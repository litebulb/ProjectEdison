// Copyright (c) Microsoft. All rights reserved.

using Edison.Devices.Onboarding.Client.Interfaces;
using Edison.Devices.Onboarding.Client.Models;
using Edison.Devices.Onboarding.Client.Services;
using Edison.Devices.Onboarding.Common.Helpers;
using Edison.Devices.Onboarding.Common.Models;
using System;
using System.Collections.Generic;
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
            AccessPointsEnumerated,
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
            _NetworksGrouping.BorderColor = (nextState == State.AccessPointConnected) ? Color.Yellow : Color.Default;
            _ConnectGrouping.BorderColor = (nextState == State.NetworksEnumerated || nextState == State.NetworkSelected) ? Color.Yellow : Color.Default;

            CurrentState = nextState;
        }

        public static IAccessPointHelper AccessPointHelper { get; set; }
        public static IDeviceApiClient DeviceApiClient { get; set; }
        private ObservableCollection<AccessPoint> _AvailableAccessPoints = new ObservableCollection<AccessPoint>();
        private ObservableCollection<Network> _AvailableNetworks = new ObservableCollection<Network>();

        public MainPage()
        {
            InitializeComponent();

            _connectButton.IsEnabled = false;

            _availableAccessPointListView.ItemsSource = _AvailableAccessPoints;
            _availableNetworkListView.ItemsSource = _AvailableNetworks;

            _clientNetworkPassword.IsEnabled = false;
            _connectClientNetwork.IsEnabled = false;
            _connectButton.IsEnabled = false;
            _requestClientNetworks.IsEnabled = false;

            _ScanApsGrouping.BorderColor = Color.Yellow;
            _NetworksGrouping.BorderColor = Color.Default;
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

        private void AccessPointHelper_ClientNetworkConnectedEvent(string status)
        {
            Device.BeginInvokeOnMainThread(() => {
                HandleState(State.NetworkConnected);
                UpdateStatus(null, status);
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

        async void WifiFindAccessPoints(object sender, EventArgs e)
        {
            UpdateStatus("WifiFindAccessPoints", "");

            _AvailableAccessPoints.Clear();
            IEnumerable<AccessPoint> accessPoints = await AccessPointHelper.FindAccessPoints();
            foreach (AccessPoint accessPoint in accessPoints)
            {
                _AvailableAccessPoints.Add(accessPoint);
            }
            HandleState(State.AccessPointsEnumerated);
            UpdateStatus("WifiFindAccessPoints", "Enumerated");
        }

        async void WifiConnectToAccessPoint(object sender, System.EventArgs e)
        {
            UpdateStatus("WifiConnectToAccessPoint", "");
            var accessPoint = _availableAccessPointListView.SelectedItem as AccessPoint;
            var result = await AccessPointHelper.ConnectToAccessPoint(accessPoint);
            if(result == WifiConnectionStatus.Connected)
                HandleState(State.AccessPointConnected);
            else
                HandleState(State.AccessPointsEnumerated);
            UpdateStatus("WifiConnectToAccessPoint", result.ToString());
        }

        async void CommandRequestClientNetworks(object sender, System.EventArgs e)
        {
            UpdateStatus("CommandRequestClientNetworks", "");

            _AvailableNetworks.Clear();
            var resultRequestGetAvailableNetworks = await DeviceApiClient.GetAvailableNetworks();
            if (resultRequestGetAvailableNetworks.IsSuccess)
            {
                foreach (string networkSsid in resultRequestGetAvailableNetworks.Networks)
                {
                    _AvailableNetworks.Add(new Network() { Ssid = networkSsid });
                }
                HandleState(State.NetworksEnumerated);
                UpdateStatus($"CommandRequestClientNetworks", string.Join(", ", resultRequestGetAvailableNetworks.Networks));
            }
            else
                UpdateStatusError($"CommandRequestClientNetworks", resultRequestGetAvailableNetworks.ErrorMessage);
        }

        async void CommandConnectClientNetwork(object sender, System.EventArgs e)
        {
            UpdateStatus("ConnectClientNetwork", "");
            var network = _availableNetworkListView.SelectedItem as Network;
            var resultRequestConnectToClientNetwork = await DeviceApiClient.ConnectToClientNetwork(new RequestCommandConnectToNetwork()
            {
                NetworkInformation = new NetworkInformation()
                {
                    Ssid = network.Ssid,
                    Password = _clientNetworkPassword.Text
                }
            });
            if (resultRequestConnectToClientNetwork.IsSuccess)
            {
                HandleState(State.NetworkConnected);
                UpdateStatus($"ConnectClientNetwork", resultRequestConnectToClientNetwork.Status);
            }
            else
                UpdateStatusError($"ConnectClientNetwork", resultRequestConnectToClientNetwork.ErrorMessage);
        }

        async void CommandDisconnectClientNetwork(object sender, System.EventArgs e)
        {
            UpdateStatus("DisconnectClientNetwork", "");
            var network = _availableNetworkListView.SelectedItem as Network;
            var resultRequestDisconnectFromClientNetwork = await DeviceApiClient.DisconnectFromClientNetwork(new RequestCommandDisconnectFromNetwork()
            {
                Ssid = network.Ssid 
            });
            if (resultRequestDisconnectFromClientNetwork.IsSuccess)
            {
                HandleState(State.NetworksEnumerated);
                UpdateStatus($"DisconnectClientNetwork", resultRequestDisconnectFromClientNetwork.Status);
            }
            else
                UpdateStatusError($"DisconnectClientNetwork", resultRequestDisconnectFromClientNetwork.ErrorMessage);
        }

        async void CommandGetDeviceId(object sender, EventArgs e)
        {
            UpdateStatus($"CommandGetDeviceId", "");
            var resultGetDevice = await DeviceApiClient.GetDeviceId();
            if(resultGetDevice.IsSuccess)
                UpdateStatus($"CommandGetDeviceId", resultGetDevice.DeviceId.ToString());
            else
                UpdateStatusError($"CommandGetDeviceId", resultGetDevice.ErrorMessage);
        }

        async void CommandListFirmwares(object sender, EventArgs e)
        {
            UpdateStatus($"CommandListFirmwares", "");
            var resultListFirmwares = await DeviceApiClient.GetFirmwares();
            if (resultListFirmwares.IsSuccess)
                UpdateStatus($"CommandListFirmwares", string.Join(", ", resultListFirmwares.Firmwares));
            else
                UpdateStatusError($"CommandListFirmwares", resultListFirmwares.ErrorMessage);
        }

        async void CommandGetAccessPointSettings(object sender, EventArgs e)
        {
            UpdateStatus($"CommandGetAccessPointSettings", "");
            var resultGetAvailableNetworks = await DeviceApiClient.GetAccessPointSettings();
            if (resultGetAvailableNetworks.IsSuccess)
                UpdateStatus($"CommandGetAccessPointSettings", $"Enabled: {resultGetAvailableNetworks.SoftAPSettings.SoftAPEnabled}, SSID: {resultGetAvailableNetworks.SoftAPSettings.SoftApSsid}, Password: {resultGetAvailableNetworks.SoftAPSettings.SoftApPassword}");
            else
                UpdateStatusError($"CommandGetAccessPointSettings", resultGetAvailableNetworks.ErrorMessage);
        }

        async void CommandSetDeviceSecretKeys(object sender, EventArgs e)
        {
            UpdateStatus($"CommandSetDeviceSecretKeys", "");

            var resultGetDevice = await DeviceApiClient.GetDeviceId();

            //Generate new keys
            string token = "IDTOKEN"; //TODO Retrieve AzureAD token from current phone session
            DeviceProvisioningRestService client = new DeviceProvisioningRestService("https://edisonapidev.eastus.cloudapp.azure.com/deviceprovisioning/", token);
            var newKeys = await client.GenerateDeviceKeys(resultGetDevice.DeviceId);

            //Pass keys to the device / Activate encryption first!!!
            var resultGetAvailableNetworks = await DeviceApiClient.SetDeviceSecretKeys(new RequestCommandSetDeviceSecretKeys() //This needs to be retrieved from REST endpoint TBD
            {
                 PortalPassword = newKeys.PortalPassword, //Will replace the password used for internal REST api
                 AccessPointPassword = newKeys.AccessPointPassword, //Will replace Access point password
                 EncryptionKey = newKeys.EncryptionKey //Will replace encryption key used to encrypt some data. After this call this becomes the current encrypt key
            });
            if (!resultGetAvailableNetworks.IsSuccess)
                UpdateStatusError($"CommandSetDeviceSecretKeys", resultGetAvailableNetworks.ErrorMessage);
            UpdateStatusError($"CommandSetDeviceSecretKeys", "Device secret keys reset.");
        }

        async void CommandProvisionDevice(object sender, EventArgs e)
        {
            UpdateStatus($"CommandProvisionDevice", "");

            //Generate CSR
            var resultGenerateCSR = await DeviceApiClient.GetGeneratedCSR();
            if (!resultGenerateCSR.IsSuccess)
            {
                UpdateStatusError($"CommandProvisionDevice", resultGenerateCSR.ErrorMessage);
                return;
            }

            if (string.IsNullOrEmpty(resultGenerateCSR.Csr))
            {
                UpdateStatusError($"CommandProvisionDevice", "Error: The CSR was empty.");
            }

            //Sign Certificate
            string token = "IDTOKEN"; //TODO Retrieve AzureAD token from current phone session
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
            var resultProvisionDevice = await DeviceApiClient.ProvisionDevice(new RequestCommandProvisionDevice() { DeviceCertificateInformation = certificate });
            if (!resultProvisionDevice.IsSuccess)
            {
                UpdateStatusError($"CommandProvisionDevice", resultProvisionDevice.ErrorMessage);
                return;
            }

            //Set Device Type
            var resultSetDeviceType = await DeviceApiClient.SetDeviceType(new RequestCommandSetDeviceType() { DeviceType = certificate.DeviceType });
            if (!resultSetDeviceType.IsSuccess)
            {
                UpdateStatusError($"CommandProvisionDevice", resultSetDeviceType.ErrorMessage);
                return;
            }

            UpdateStatus($"CommandProvisionDevice", "Provisionning complete");
        }
        
        void Exit(object sender, EventArgs e)
        {
            AccessPointHelper.Disconnect();
        }
    }
}
