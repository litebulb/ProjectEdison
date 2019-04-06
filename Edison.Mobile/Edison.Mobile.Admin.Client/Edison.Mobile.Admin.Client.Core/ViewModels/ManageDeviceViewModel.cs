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
using System.Linq;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class ManageDeviceViewModel : DeviceSetupBaseViewModel
    {
        public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed;
        public event EventHandler<CheckingConnectionStatusUpdatedEventArgs> CheckingConnectionStatusUpdated;
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
            IDeviceRestService deviceRestService,
            DeviceProvisioningRestService deviceProvisioningRestService,
            IOnboardingRestService onboardingRestService,
            IWifiService wifiService
        ) : base(deviceSetupService, deviceProvisioningRestService, onboardingRestService, wifiService)
        {
            this.locationService = locationService;
            this.deviceRestService = deviceRestService;

        }

        public override void BindEventHandlers()
        {
            this.wifiService.CheckingConnectionStatusUpdated += WifiService_CheckingConnectionStatusUpdated;
            this.wifiService.ConnectionFailed += WifiService_ConnectionFailed;
            base.BindEventHandlers();
        }

        public override void UnBindEventHandlers()
        {
            this.wifiService.CheckingConnectionStatusUpdated -= WifiService_CheckingConnectionStatusUpdated;
            this.wifiService.ConnectionFailed -= WifiService_ConnectionFailed;
            base.UnBindEventHandlers();
        }


        private void WifiService_ConnectionFailed(object sender, Common.WiFi.ConnectionFailedEventArgs e)
        {
            CheckingConnectionStatusUpdated?.Invoke(this, new CheckingConnectionStatusUpdatedEventArgs("Could not connect to device"));
        }

        private void WifiService_CheckingConnectionStatusUpdated(object sender, Common.WiFi.CheckingConnectionStatusUpdatedEventArgs e)
        {
            if (e.IsConnected)
            {
                if (DeviceSetupService.SSIDIsEdisonDevice(e.SSID))
                {
                    onboardingRestService.SetBasicAuthentication(deviceSetupService.PortalPassword);
                    Task.Run(async () =>
                    {
                        var networks = await this.onboardingRestService.GetAvailableWifiNetworks();

                        if (networks != default(IEnumerable<Models.AvailableNetwork>))
                        {
                            var connectedNetwork = networks.FirstOrDefault(i => i.AlreadyConnected);

                            if (connectedNetwork != default(Models.AvailableNetwork))
                            {
                                this.wifiService.CheckingConnectionStatusUpdated -= WifiService_CheckingConnectionStatusUpdated;
                                this.deviceSetupService.ConnectedWifiSSID = connectedNetwork.SSID;
                                CheckingConnectionStatusUpdated?.Invoke(this, new CheckingConnectionStatusUpdatedEventArgs(connectedNetwork.SSID));
                            }
                            else
                            {
                                this.wifiService.CheckingConnectionStatusUpdated -= WifiService_CheckingConnectionStatusUpdated;
                                CheckingConnectionStatusUpdated?.Invoke(this, new CheckingConnectionStatusUpdatedEventArgs("Device Not Using WiFi"));
                            }
                        }
                        else
                        {
                            this.wifiService.CheckingConnectionStatusUpdated -= WifiService_CheckingConnectionStatusUpdated;
                            CheckingConnectionStatusUpdated?.Invoke(this, new CheckingConnectionStatusUpdatedEventArgs("Device Not Connected To a Network"));
                        }

                        await wifiService.DisconnectFromWifiNetwork(new WifiNetwork() { SSID = e.SSID });
                    });
                }
                else
                {
                    Task.Run(async () =>
                    {
                        await CompleteUpdate();
                    });
                }
            }

        }

        public async Task GetDeviceNetworkInfo()
        {
            originalWifiNetwork = await wifiService.GetCurrentlyConnectedWifiNetwork();
            
            if (!DeviceSetupService.SSIDIsEdisonDevice(originalWifiNetwork.SSID))
            {
                deviceSetupService.OriginalSSID = originalWifiNetwork.SSID;
                await wifiService.ConnectToWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork.SSID, deviceSetupService.WiFiPassword);
            }
            else
            {
                // wifi should already be defined
                CheckingConnectionStatusUpdated?.Invoke(this, new CheckingConnectionStatusUpdatedEventArgs(this.deviceSetupService.ConnectedWifiSSID));
            }
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
            var network = await wifiService.GetCurrentlyConnectedWifiNetwork();

            if(DeviceSetupService.SSIDIsEdisonDevice(network.SSID))
            {
                await wifiService.ConnectToWifiNetwork(deviceSetupService.OriginalSSID);
            }
            else
            {
                await CompleteUpdate();
            }

           
        }

        private async Task CompleteUpdate()
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
                SSID = deviceSetupService.CurrentDeviceModel.SSID
            };

            var success = await deviceRestService.UpdateDevice(updateTagsModel);

            deviceSetupService.IsNew = false;

            OnDeviceUpdated?.Invoke(this, success);
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            Task.Run(async () =>  await GetNearDevices());
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
            if (!deviceSetupService.IsNew)
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
        }
        
        public async Task<DeviceModel> GetDevice(Guid deviceId)
        {
            return await deviceRestService.GetDevice(deviceId);
        }
    }
    
}
