using System;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Admin.Client.Core.Shared;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class ChooseDeviceTypeViewModel : BaseViewModel
    {
        readonly DeviceSetupService deviceSetupService;

        public ChooseDeviceTypeViewModel(DeviceSetupService deviceSetupService)
        {
            this.deviceSetupService = deviceSetupService;
        }

        public void SetDeviceType(DeviceType deviceType)
        {
            deviceSetupService.ClearDevice();
            deviceSetupService.SetDeviceType(deviceType);
        }
    }
}
