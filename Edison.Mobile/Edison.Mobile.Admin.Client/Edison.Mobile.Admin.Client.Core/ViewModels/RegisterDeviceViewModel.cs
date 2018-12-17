using System;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Admin.Client.Core.Shared;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class RegisterDeviceViewModel : BaseViewModel
    {
        readonly DeviceSetupService deviceSetupService;

        public string DeviceTypeAsString => deviceSetupService.DeviceTypeAsString;

        public RegisterDeviceViewModel(DeviceSetupService deviceSetupService)
        {
            this.deviceSetupService = deviceSetupService;
        }
    }
}
