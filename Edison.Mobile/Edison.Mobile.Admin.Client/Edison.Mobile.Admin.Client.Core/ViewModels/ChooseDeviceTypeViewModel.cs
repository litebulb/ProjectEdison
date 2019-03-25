using System;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Admin.Client.Core.Shared;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class ChooseDeviceTypeViewModel : DeviceSetupBaseViewModel
    {
        public ChooseDeviceTypeViewModel(DeviceSetupService deviceSetupService,
            DeviceProvisioningRestService deviceProvisioningRestService,
            IOnboardingRestService onboardingRestService,
            IWifiService wifiService)
            : base(deviceSetupService, deviceProvisioningRestService, onboardingRestService, wifiService)
        {
        }

        public void SetDeviceType(DeviceType deviceType)
        {
            deviceSetupService.ClearDevice();
            deviceSetupService.CurrentDeviceModel.DeviceType = DeviceSetupService.DeviceTypeToString(deviceType);
            deviceSetupService.IsNew = true;
        }
    }
}
