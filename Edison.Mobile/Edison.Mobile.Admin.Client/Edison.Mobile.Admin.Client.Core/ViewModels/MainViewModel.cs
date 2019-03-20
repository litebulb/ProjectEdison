using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class MainViewModel : DeviceSetupBaseViewModel
    {
        readonly IDeviceRestService deviceRestService;
        readonly AuthService authService;

        public ObservableRangeCollection<DeviceModel> NearDevices { get; private set; } = new ObservableRangeCollection<DeviceModel>();

        public MainViewModel(DeviceSetupService deviceSetupService, 
            IDeviceRestService deviceRestService, AuthService authService,
            DeviceProvisioningRestService deviceProvisioningRestService,
            IOnboardingRestService onboardingRestService,
            IWifiService wifiService
        ) : base(deviceSetupService, deviceProvisioningRestService, onboardingRestService, wifiService)
        {
            this.deviceRestService = deviceRestService;
            this.authService = authService;
        }

        public override async void ViewAppeared()
        {
            base.ViewAppeared();

            await GetNearDevices();
        }

        async Task GetNearDevices()
        {
            var devices = await deviceRestService.GetDevices();
            if (devices != null)
            {
                NearDevices.ReplaceRange(devices);
            }
        }
        
        public async Task SignOut()
        {
            await authService.SignOut();
        }
    }
}
