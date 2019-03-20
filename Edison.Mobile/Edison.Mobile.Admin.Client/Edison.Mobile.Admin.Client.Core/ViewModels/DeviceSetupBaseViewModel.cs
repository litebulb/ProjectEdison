using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class DeviceSetupBaseViewModel : BaseViewModel
    {
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
            deviceSetupService.WiFiPassword = keys.AccessPointPassword;
        }

        public async Task GetDeviceNetworkInfo()
        {
            var originalNetwork = await wifiService.GetCurrentlyConnectedWifiNetwork();

            await wifiService.DisconnectFromWifiNetwork(originalNetwork);
            var success = await wifiService.ConnectToWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork.SSID, deviceSetupService.WiFiPassword);
            onboardingRestService.SetBasicAuthentication(deviceSetupService.PortalPassword);

            await Task.Delay(4000);

            var networks = await this.onboardingRestService.GetAvailableWifiNetworks();

            if (networks != default(IEnumerable<Models.AvailableNetwork>))
            {
                var connectedNetwork = networks.FirstOrDefault(i => i.AlreadyConnected);

                if (connectedNetwork != default(Models.AvailableNetwork))
                {
                    this.deviceSetupService.ConnectedWifiSSID = connectedNetwork.SSID;
                }
            }
            await wifiService.ConnectToWifiNetwork(originalNetwork.SSID);
        }

        public string GetConnectedSSID()
        {
            return this.deviceSetupService.ConnectedWifiSSID;
        }
    }
}
