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
        

        protected  WifiNetwork originalWifiNetwork;

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
            deviceSetupService.WiFiPassword = keys.SSIDPassword;
        }

        public string GetConnectedSSID()
        {
            return this.deviceSetupService.ConnectedWifiSSID;
        }
    }
}
