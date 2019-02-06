using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class ManageDeviceViewModel : DeviceSetupBaseViewModel
    {
        readonly ILocationService locationService;
        readonly DeviceRestService deviceRestService;

        public bool IsOnboardingStepSix { get; set; }

        public ManageDeviceViewModel(
            DeviceSetupService deviceSetupService,
            ILocationService locationService,
            DeviceRestService deviceRestService
        ) : base(deviceSetupService)
        {
            this.locationService = locationService;
            this.deviceRestService = deviceRestService;
        }

        public void AddCustomDeviceField(string key, string value)
        {
            deviceSetupService.AddCustomDeviceField(key, value);
        }

        public async Task<EdisonLocation> GetLastKnownLocation()
        {
            return await locationService.GetLastKnownLocationAsync();
        }

        public async Task UpdateDevice()
        {
            
        }
    }
}
