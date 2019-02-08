using System;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Admin.Client.Core.Shared;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class ChooseDeviceTypeViewModel : DeviceSetupBaseViewModel
    {
        public ChooseDeviceTypeViewModel(DeviceSetupService deviceSetupService)
            : base(deviceSetupService)
        {
        }

        public void SetDeviceType(DeviceType deviceType)
        {
            deviceSetupService.ClearDevice();
            deviceSetupService.CurrentDeviceModel.DeviceType = DeviceSetupService.DeviceTypeToString(deviceType);
        }
    }
}
