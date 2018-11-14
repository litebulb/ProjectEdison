// Copyright (c) Microsoft. All rights reserved.

//#define AUTOMATE_FOR_TESTING
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
#if AUTOMATE_FOR_TESTING
        private string _TestAutomation_AccessPointPartial = "";
        private string _TestAutomation_NetworkPartial = "";
        private string _TestAutomation_NetworkPassword = "";
#endif
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

        public static IAccessPointHelper AccessPointHelper { get; set; }
        public static INetworkCommandsHelper NetworkCommandsHelper { get; set; }
        public static IDeviceConfigurationCommandsHelper DeviceConfigurationHelper { get; set; }
        public static IDeviceProvisionCommandsHelper DeviceProvisionCommandsHelper { get; set; }
        private ObservableCollection<AccessPoint> _AvailableAccessPoints = new ObservableCollection<AccessPoint>();
        private ObservableCollection<Network> _AvailableNetworks = new ObservableCollection<Network>();

        private string _StatusAction = "";
        private string _StatusResult = "";


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
            NetworkCommandsHelper.ClientNetworkConnectedEvent += AccessPointHelper_ClientNetworkConnectedEvent;
            NetworkCommandsHelper.ClientNetworksEnumeratedEvent += AccessPointHelper_ClientNetworksEnumeratedEvent;

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

            _StatusAction = "";
            _StatusResult = "";

#if AUTOMATE_FOR_TESTING
            AccessPointHelper.FindAccessPoints(_AvailableAccessPoints);
#endif
        }

        private void UpdateStatus(string action, string result)
        {
            if (action != null) _StatusAction = action;
            if (result != null) _StatusResult = result;
            _Status.Text = string.Format("{0} ... {1}", _StatusAction, _StatusResult);
        }

        private void AccessPointHelper_ClientNetworksEnumeratedEvent(string status)
        {
            Device.BeginInvokeOnMainThread(() => {

                HandleState(State.NetworksEnumerated);
                UpdateStatus(null, status);

#if AUTOMATE_FOR_TESTING
                foreach (var n in _AvailableNetworks)
                {
                    if (n.Ssid.Contains(_TestAutomation_NetworkPartial))
                    {
                        AccessPointHelper.ConnectToClientNetwork(n.Ssid.ToString(), _TestAutomation_NetworkPassword);
                        break;
                    }
                }
#endif
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

#if AUTOMATE_FOR_TESTING
                foreach (var ap in _AvailableAccessPoints)
                {
                    if (ap.Ssid.Contains(_TestAutomation_AccessPointPartial))
                    {
                        _availableAccessPointListView.SelectedItem = ap;
                        AccessPointHelper.ConnectToAccessPoint(ap);
                        break;
                    }
                }
#endif
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

        void HandleException(AggregateException exception)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                HandleState(State.Started);
                UpdateStatus(null, string.Format("Encountered problem: {0}", exception.Message));
            });
        }

        void ConnectClientNetwork(object sender, System.EventArgs e)
        {
            UpdateStatus("Connecting to client network", "");
            var network = _availableNetworkListView.SelectedItem as Network;
            var task = NetworkCommandsHelper.ConnectToClientNetwork(network.Ssid, _clientNetworkPassword.Text);
            task.ContinueWith(t => { HandleException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        void DisconnectClientNetwork(object sender, System.EventArgs e)
        {
            UpdateStatus("Disconnecting from client network", "");
            var network = _availableNetworkListView.SelectedItem as Network;
            var task = NetworkCommandsHelper.DisconnectFromClientNetwork(network.Ssid);
            task.ContinueWith(t => { HandleException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        void ConnectToAccessPoint(object sender, System.EventArgs e)
        {
            UpdateStatus("Connecting to access point", "");
            var accessPoint = _availableAccessPointListView.SelectedItem as AccessPoint;
            var task = AccessPointHelper.ConnectToAccessPoint(accessPoint);
            task.ContinueWith(t => { HandleException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void RequestClientNetworks(object sender, System.EventArgs e)
        {
            UpdateStatus("Getting available client networks", "");
            var task = NetworkCommandsHelper.RequestClientNetworks(_AvailableNetworks);
            task.ContinueWith(t => { HandleException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void FindAccessPoints(object sender, System.EventArgs e)
        {
            UpdateStatus("Getting available access points", "");
            var task = AccessPointHelper.FindAccessPoints(_AvailableAccessPoints);
            task.ContinueWith(t => { HandleException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void DebugConnectToServer(object sender, System.EventArgs e)
        {
            UpdateStatus($"Connecting to Stream Server", SharedConstants.DEBUG_NETWORK_IP);
            var task = AccessPointHelper.DebugConnectToStreamServer();
            _getDeviceId.IsEnabled = true;
            task.ContinueWith(t => { HandleException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public async void DebugGetDeviceId(object sender, System.EventArgs e)
        {
            UpdateStatus($"GetDeviceId", "");
            string deviceId = await DeviceProvisionCommandsHelper.GetDeviceId();
            UpdateStatus($"GetDeviceId", deviceId);
           // task.ContinueWith(t => { HandleException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// Provision device using device Id. Use CSR Instead
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void DebugProvisionSmartBulbCsr(object sender, System.EventArgs e)
        {
            UpdateStatus($"DebugProvisionSmartBulbCsr", "");
            string csr = await DeviceProvisionCommandsHelper.GenerateCSR();

            //Generate Certificate
            //TODO Retrieve AzureAD token from current phone session
            string token = "ADTOKEN";
            DeviceProvisioningRestService client = new DeviceProvisioningRestService
                ("http://localhost:49563/", token);
            //("https://edisonapidev.eastus.cloudapp.azure.com/deviceprovisioning/", token);
            var certificate = await client.GenerateCertificate(new DeviceCertificateRequestModel()
            {
                DeviceType = "Edison.Devices.SmartBulb",
                Csr = csr
            });

            bool result = false;
            if (certificate != null)
                result = await DeviceProvisionCommandsHelper.ProvisionDevice(certificate);

            if(result)
                result = await DeviceConfigurationHelper.SetDeviceType(certificate.DeviceType);

            UpdateStatus($"DebugProvisionSmartBulbCsr", result.ToString());
            // task.ContinueWith(t => { HandleException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// Provision device using CSR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void DebugProvisionSoundSensorCsr(object sender, System.EventArgs e)
        {
            UpdateStatus($"DebugProvisionSoundSensorCsr", "");
            string csr = await DeviceProvisionCommandsHelper.GenerateCSR();

            //Generate Certificate
            //TODO Retrieve AzureAD token from current phone session
            string token = "ADTOKEN";
            DeviceProvisioningRestService client = new DeviceProvisioningRestService
                //("http://localhost:49563/", token);
            ("https://edisonapidev.eastus.cloudapp.azure.com/deviceprovisioning/", token);
            var certificate = await client.GenerateCertificate(new DeviceCertificateRequestModel()
            {
                DeviceType = "Edison.Devices.SoundSensor",
                Csr = csr
            });

            bool result = false;
            if (certificate != null)
                result = await DeviceProvisionCommandsHelper.ProvisionDevice(certificate);

            if (result)
                result = await DeviceConfigurationHelper.SetDeviceType(certificate.DeviceType);

            UpdateStatus($"DebugProvisionSoundSensorCsr", result.ToString());
            // task.ContinueWith(t => { HandleException(t.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void Exit(object sender, System.EventArgs e)
        {
            AccessPointHelper.Disconnect();
        }
    }
}
