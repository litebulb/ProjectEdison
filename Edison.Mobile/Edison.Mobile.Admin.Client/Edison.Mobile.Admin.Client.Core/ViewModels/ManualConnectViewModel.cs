using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Core.Models;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class ManualConnectViewModel : BaseViewModel
    {
        readonly DeviceSetupService deviceSetupService;
        readonly IWifiService wifiService;
        readonly OnboardingRestService onboardingRestService;
        readonly DeviceProvisioningRestService deviceProvisioningRestService;

        public string DeviceTypeAsString => deviceSetupService.DeviceTypeAsFriendlyString;

        public ObservableRangeCollection<WifiNetwork> AvailableWifiNetworks { get; } = new ObservableRangeCollection<WifiNetwork>();

        public ManualConnectViewModel(
            DeviceSetupService deviceSetupService,
            IWifiService wifiService,
            OnboardingRestService onboardingRestService,
            DeviceProvisioningRestService deviceProvisioningRestService
        )
        {
            this.deviceSetupService = deviceSetupService;
            this.wifiService = wifiService;
            this.onboardingRestService = onboardingRestService;
            this.deviceProvisioningRestService = deviceProvisioningRestService;
        }

        public override async void ViewAppeared()
        {
            base.ViewAppeared();

            var wifiNetworks = await wifiService.GetAvailableWifiNetworks();
            AvailableWifiNetworks.AddRange(wifiNetworks);
        }

        public async Task<bool> ProvisionDevice(WifiNetwork wifiNetwork)
        {
            var defaultWifiNetwork = await wifiService.GetCurrentlyConnectedWifiNetwork();

            // connect to device
            var success = await wifiService.ConnectToWifiNetwork(wifiNetwork.SSID, deviceSetupService.DefaultPassword);

            await Task.Delay(2000);

            if (!success) return false;

            deviceSetupService.CurrentDeviceHotspotNetwork = success ? wifiNetwork : null;

            // get stuff from device
            var deviceId = await onboardingRestService.GetDeviceId();
            var csrResult = await onboardingRestService.GetGeneratedCSR();

            // disconnect from device and connect to the internet to provision device with services
            await wifiService.DisconnectFromWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork);

            await Task.Delay(2000);

            var certificate = await deviceProvisioningRestService.GenerateDeviceCertificate(new DeviceCertificateRequestModel
            {
                Csr = csrResult?.Csr ?? "MIIBbjCB2AIBADAvMS0wKwYDVQQDEyQ4OTJlYWM5YS1iOWFkLTQ0NDgtYWEwYS0wOTI0MDE1YWMwMWEwgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGBALeqOH+XoeXXERg8neKzr3IumxTDMKsPzKjZ/kfE1gu/FHmr1ugPuRTtQzP5WFVD5lWqtEKJyX+YDCjNevKeHBSpHTAAdVR8GbpDdvRvij0k6yrmrjTRVohO5bTaE611KNzXOW5K4Y8PhoTHasNnMEydfAh4ysut92lWObmg2CG1AgMBAAGgADANBgkqhkiG9w0BAQsFAAOBgQCg8dbM4gMxChp4MF67B/0ARv5Ezq3423v/Tkj5KOMxFql+NeYtM9JpIWABMw2xlARl+agp9e8eaj503grhHjYeGV0afC2/8AA2o/PyZOrS80QViDK6Z4cY+zUO5hp3darGCEH14fuAHKwrokSQxYReqdBELyT3r4ZnCdbi+NUx7A==",
                DeviceType = deviceSetupService.DeviceTypeAsString,
            });

            Console.WriteLine(certificate);

            var reconnectSuccess = await wifiService.ConnectToWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork.SSID, deviceSetupService.DefaultPassword);

            await Task.Delay(2000);

            if (!reconnectSuccess) return false;

            var setDeviceTypeResult = await onboardingRestService.SetDeviceType(new RequestCommandSetDeviceType
            {
                DeviceType = certificate.DeviceType,
            });

            return setDeviceTypeResult.IsSuccess;
        }
    }
}
