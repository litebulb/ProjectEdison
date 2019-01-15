using System;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Core.Models;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Admin.Client.Core.Shared;
using Edison.Mobile.Common.Shared;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class RegisterDeviceViewModel : BaseViewModel
    {
        readonly DeviceSetupService deviceSetupService;
        readonly IWifiService wifiService;
        readonly OnboardingRestService onboardingRestService;
        readonly DeviceProvisioningRestService deviceProvisioningRestService;

        public event ViewNotification OnBeginDevicePairing;
        public event EventHandler<OnFinishDevicePairingEventArgs> OnFinishDevicePairing;

        public class OnFinishDevicePairingEventArgs : EventArgs
        {
            public bool IsSuccess { get; set; }
        }

        public string DeviceTypeAsString => deviceSetupService.DeviceTypeAsFriendlyString;

        public string MockDeviceID => "BA27EB910E94";

        public RegisterDeviceViewModel(
            DeviceSetupService deviceSetupService, 
            IWifiService wifiService,
            OnboardingRestService onboardingRestService,
            DeviceProvisioningRestService deviceProvisioningRestService
        )
        {
            this.deviceSetupService = deviceSetupService;
            this.wifiService = wifiService;
        }

        bool ProvisionDeviceFail()
        {
            OnFinishDevicePairing?.Invoke(this, new OnFinishDevicePairingEventArgs
            {
                IsSuccess = false,
            });

            return false;
        }

        public async Task<bool> ProvisionDevice(WifiNetwork wifiNetwork)
        {
            OnBeginDevicePairing?.Invoke();

            var defaultWifiNetwork = await wifiService.GetCurrentlyConnectedWifiNetwork();

            // connect to device
            var success = await wifiService.ConnectToWifiNetwork(wifiNetwork.SSID, deviceSetupService.DefaultPassword);

            await Task.Delay(2000);

            if (!success) ProvisionDeviceFail();

            deviceSetupService.CurrentDeviceHotspotNetwork = success ? wifiNetwork : null;

            // get stuff from device
            var deviceId = await onboardingRestService.GetDeviceId();
            var csrResult = await onboardingRestService.GetGeneratedCSR();

            // disconnect from device and connect to the internet to provision device with services
            await wifiService.DisconnectFromWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork);

            await Task.Delay(2000);

            if (csrResult == null) ProvisionDeviceFail();

            // provision device with azure
            var certificate = await deviceProvisioningRestService.GenerateDeviceCertificate(new DeviceCertificateRequestModel
            {
                Csr = csrResult?.Csr ?? "MIIBbjCB2AIBADAvMS0wKwYDVQQDEyQ4OTJlYWM5YS1iOWFkLTQ0NDgtYWEwYS0wOTI0MDE1YWMwMWEwgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGBALeqOH+XoeXXERg8neKzr3IumxTDMKsPzKjZ/kfE1gu/FHmr1ugPuRTtQzP5WFVD5lWqtEKJyX+YDCjNevKeHBSpHTAAdVR8GbpDdvRvij0k6yrmrjTRVohO5bTaE611KNzXOW5K4Y8PhoTHasNnMEydfAh4ysut92lWObmg2CG1AgMBAAGgADANBgkqhkiG9w0BAQsFAAOBgQCg8dbM4gMxChp4MF67B/0ARv5Ezq3423v/Tkj5KOMxFql+NeYtM9JpIWABMw2xlARl+agp9e8eaj503grhHjYeGV0afC2/8AA2o/PyZOrS80QViDK6Z4cY+zUO5hp3darGCEH14fuAHKwrokSQxYReqdBELyT3r4ZnCdbi+NUx7A==",
                DeviceType = deviceSetupService.DeviceTypeAsString,
            });

            // reconnect to device to set device type
            var reconnectSuccess = await wifiService.ConnectToWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork.SSID, deviceSetupService.DefaultPassword);

            await Task.Delay(2000);

            if (!reconnectSuccess) ProvisionDeviceFail();

            var setDeviceTypeResult = await onboardingRestService.SetDeviceType(new RequestCommandSetDeviceType
            {
                DeviceType = certificate.DeviceType,
            });

            OnFinishDevicePairing?.Invoke(this, new OnFinishDevicePairingEventArgs
            {
                IsSuccess = setDeviceTypeResult.IsSuccess,
            });

            return setDeviceTypeResult.IsSuccess;
        }
    }
}
