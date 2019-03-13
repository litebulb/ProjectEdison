using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class ManageDeviceViewModel : DeviceSetupBaseViewModel
    {
        readonly ILocationService locationService;
        readonly IDeviceRestService deviceRestService;

        public ObservableRangeCollection<DeviceModel> NearDevices { get; private set; } = new ObservableRangeCollection<DeviceModel>();

        public bool IsOnboardingStepSix { get; set; }

        public event EventHandler<bool> OnDeviceUpdated;
        
        public class OnDeviceLoadedEventArgs : EventArgs
        {
            public DeviceModel DeviceModel { get; set; }
            
            public OnDeviceLoadedEventArgs(DeviceModel deviceModel)
            {
                this.DeviceModel = deviceModel;
            }
        }
        
        public event EventHandler<OnDeviceLoadedEventArgs> OnDeviceLoaded;
        public event EventHandler<EventArgs> OnDeviceLoadFail;
        
        public ManageDeviceViewModel(
            DeviceSetupService deviceSetupService,
            ILocationService locationService,
            IDeviceRestService deviceRestService
        ) : base(deviceSetupService)
        {
            this.locationService = locationService;
            this.deviceRestService = deviceRestService;            
        }

        public override void ViewCreated()
        {
            base.ViewCreated();

            Task.Run(async () =>
            {
                await LoadDevice(this.CurrentDeviceModel.DeviceId);
            });            
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
            var currentDeviceModel = deviceSetupService.CurrentDeviceModel;
            if (currentDeviceModel == null) return;

             var updateTagsModel = new DevicesUpdateTagsModel
            { 
                DeviceIds = new List<Guid> { currentDeviceModel.DeviceId },
                Name = currentDeviceModel.Name,
                Enabled = currentDeviceModel.Enabled,
                Geolocation = currentDeviceModel.Geolocation,
                Location1 = currentDeviceModel.Location1,
                Location2 = currentDeviceModel.Location2,
                Location3 = currentDeviceModel.Location3,      
                SSID = currentDeviceModel.SSID
            };

            var success = await deviceRestService.UpdateDevice(updateTagsModel);

            OnDeviceUpdated?.Invoke(this, success);
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
        
        public async Task LoadDevice(Guid deviceId)
        {
            var device = await GetDevice(deviceId);
           
            if (device == null)
            {
                OnDeviceLoadFail.Invoke(this, EventArgs.Empty);
            }
            else
            {
                deviceSetupService.CurrentDeviceModel = device;
                OnDeviceLoaded.Invoke(this, new OnDeviceLoadedEventArgs(device));
            }
        }
        
        public async Task<DeviceModel> GetDevice(Guid deviceId)
        {
            return await deviceRestService.GetDevice(deviceId);
        }
    }
    
}
