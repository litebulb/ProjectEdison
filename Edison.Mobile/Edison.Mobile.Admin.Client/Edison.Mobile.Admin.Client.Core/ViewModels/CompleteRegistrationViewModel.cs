using System;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class CompleteRegistrationViewModel : DeviceSetupBaseViewModel
    {
        public CompleteRegistrationViewModel(DeviceSetupService deviceSetupService) : base(deviceSetupService)
        {
        }
    }
}
