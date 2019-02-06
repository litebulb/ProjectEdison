using System;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class DeviceSetupBaseViewModel : BaseViewModel
    {
        protected readonly DeviceSetupService deviceSetupService;

        public DeviceSetupBaseViewModel(DeviceSetupService deviceSetupService)
        {
            this.deviceSetupService = deviceSetupService;
        }

        public string DeviceTypeAsString => deviceSetupService.DeviceTypeAsFriendlyString;
    }
}
