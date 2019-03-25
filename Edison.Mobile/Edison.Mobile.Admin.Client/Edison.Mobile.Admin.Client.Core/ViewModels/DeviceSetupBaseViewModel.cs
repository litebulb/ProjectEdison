using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class ConnectionFailedEventArgs :EventArgs
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
        public CheckingConnectionStatusUpdatedEventArgs(string statusText)
        {
            this.StatusText = statusText;
        }
    }

    public class DeviceSetupBaseViewModel : BaseViewModel
    {
        
        public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed;
        public event EventHandler<CheckingConnectionStatusUpdatedEventArgs> CheckingConnectionStatusUpdated;

        protected readonly DeviceSetupService deviceSetupService;
        protected readonly DeviceProvisioningRestService deviceProvisioningRestService;
        protected readonly IOnboardingRestService onboardingRestService;
        protected readonly IWifiService wifiService;        

        public DeviceSetupBaseViewModel(DeviceSetupService deviceSetupService,
            DeviceProvisioningRestService deviceProvisioningRestService,
            IOnboardingRestService onboardingRestService,
            IWifiService wifiService)
        {
            this.deviceSetupService = deviceSetupService;
            this.deviceProvisioningRestService = deviceProvisioningRestService;
            this.onboardingRestService = onboardingRestService;
            this.wifiService = wifiService;

            this.wifiService.CheckingConnectionStatusUpdated += WifiService_CheckingConnectionStatusUpdated;
        }

        private void WifiService_CheckingConnectionStatusUpdated(object sender, Common.WiFi.CheckingConnectionStatusUpdatedEventArgs e)
        {
            CheckingConnectionStatusUpdated?.Invoke(this, new CheckingConnectionStatusUpdatedEventArgs(e.StatusText));
        }

        public string DeviceTypeAsString => deviceSetupService.DeviceTypeAsFriendlyString;

        public DeviceModel CurrentDeviceModel
        {
            get => deviceSetupService.CurrentDeviceModel;
        }

        public void SetDeviceModel(DeviceModel model)
        {
            deviceSetupService.CurrentDeviceHotspotNetwork.SSID = model.SSID;
            deviceSetupService.CurrentDeviceModel = model;
        }

        public async Task SetKeys()
        {
            var keys = await deviceProvisioningRestService.GetDeviceKeys(CurrentDeviceModel.DeviceId);
            deviceSetupService.PortalPassword = keys.PortalPassword;
            deviceSetupService.WiFiPassword = keys.AccessPointPassword;
        }

        public async Task GetDeviceNetworkInfo()
        {           
            var originalNetwork = await wifiService.GetCurrentlyConnectedWifiNetwork();

            if(!DeviceSetupService.SSIDIsEdisonDevice(originalNetwork.SSID))
            {
                var success = await wifiService.ConnectToWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork.SSID);

                if (!success)
                {
                    success = await wifiService.ConnectToWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork.SSID, deviceSetupService.WiFiPassword);
                }

                if (!success)
                {
                    CheckingConnectionStatusUpdated?.Invoke(this, new CheckingConnectionStatusUpdatedEventArgs("Unable To Connect To Device"));
                    return;
                }
            }


            onboardingRestService.SetBasicAuthentication(deviceSetupService.PortalPassword);
            
            var networks = await this.onboardingRestService.GetAvailableWifiNetworks();

            if (networks != default(IEnumerable<Models.AvailableNetwork>))
            {
                var connectedNetwork = networks.FirstOrDefault(i => i.AlreadyConnected);

                if (connectedNetwork != default(Models.AvailableNetwork))
                {
                    this.wifiService.CheckingConnectionStatusUpdated -= WifiService_CheckingConnectionStatusUpdated;
                    this.deviceSetupService.ConnectedWifiSSID = connectedNetwork.SSID;
                    CheckingConnectionStatusUpdated?.Invoke(this, new CheckingConnectionStatusUpdatedEventArgs(connectedNetwork.SSID));                    
                }
            }
            else
            {
                this.wifiService.CheckingConnectionStatusUpdated -= WifiService_CheckingConnectionStatusUpdated;
                CheckingConnectionStatusUpdated?.Invoke(this, new CheckingConnectionStatusUpdatedEventArgs("Device Not Connected To a Network"));
            }
            await wifiService.ConnectToWifiNetwork(originalNetwork.SSID);
        }

        public string GetConnectedSSID()
        {
            return this.deviceSetupService.ConnectedWifiSSID;
        }
    }
}
