using System;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Admin.Client.Core.Shared;
using Edison.Mobile.Common.Shared;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class RegisterDeviceViewModel : BaseViewModel
    {
        readonly DeviceSetupService deviceSetupService;
        readonly IWifiService wifiService;

        public string DeviceTypeAsString => deviceSetupService.DeviceTypeAsString;

        public event ViewNotification OnPairedSuccessfully;

        public RegisterDeviceViewModel(DeviceSetupService deviceSetupService, IWifiService wifiService)
        {
            this.deviceSetupService = deviceSetupService;
            this.wifiService = wifiService;
        }

        public async override void ViewAppeared()
        {
            base.ViewAppeared();

            if (deviceSetupService.CurrentWifiNetwork == null)
            {
                var wifiNetwork = await wifiService.GetCurrentlyConnectedWifiNetwork();
                deviceSetupService.CurrentWifiNetwork = wifiNetwork;
            }

            if (deviceSetupService.CurrentDeviceHotspotNetwork != null) // user paired manually and we've now returned to this view
            {
                OnPairedSuccessfully?.Invoke();
            }
        }

        public void SetDeviceHotspotNetwork(WifiNetwork wifiNetwork)
        {
            deviceSetupService.CurrentDeviceHotspotNetwork = wifiNetwork;
            OnPairedSuccessfully?.Invoke();
        }
    }
}
