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
        public event EventHandler<string> OnPairingStatusTextChanged;

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
            this.onboardingRestService = onboardingRestService;
            this.deviceProvisioningRestService = deviceProvisioningRestService;
        }

        bool ProvisionDeviceFail()
        {
            OnFinishDevicePairing?.Invoke(this, new OnFinishDevicePairingEventArgs
            {
                IsSuccess = false,
            });

            return false;
        }

        void SetPairingStatusText(string statusText)
        {
            OnPairingStatusTextChanged?.Invoke(this, statusText);
        }

        public async Task<bool> ProvisionDevice(WifiNetwork wifiNetwork)
        {
            OnBeginDevicePairing?.Invoke();

            // connect to device
            SetPairingStatusText("Connecting to device...");

            var defaultWifiNetwork = await wifiService.GetCurrentlyConnectedWifiNetwork();
            var success = await wifiService.ConnectToWifiNetwork(wifiNetwork.SSID, deviceSetupService.DefaultPassword);

            await Task.Delay(2000);

            if (!success) ProvisionDeviceFail();


            // get stuff from device
            SetPairingStatusText("Grabbing some information from the device...");

            deviceSetupService.CurrentDeviceHotspotNetwork = success ? wifiNetwork : null;

            var deviceId = await onboardingRestService.GetDeviceId();
            var csrResult = await onboardingRestService.GetGeneratedCSR();

            // disconnect from device and connect to the internet to provision device with services
            await wifiService.DisconnectFromWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork);

            await Task.Delay(2000);

            if (csrResult == null) ProvisionDeviceFail();


            // provision device with azure
            SetPairingStatusText("Provisioning device...");

            var certificateResponse = await deviceProvisioningRestService.GenerateDeviceCertificate(new DeviceCertificateRequestModel
            {
                Csr = csrResult?.Csr ?? "MIIBbjCB2AIBADAvMS0wKwYDVQQDEyQ4OTJlYWM5YS1iOWFkLTQ0NDgtYWEwYS0wOTI0MDE1YWMwMWEwgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGBALeqOH+XoeXXERg8neKzr3IumxTDMKsPzKjZ/kfE1gu/FHmr1ugPuRTtQzP5WFVD5lWqtEKJyX+YDCjNevKeHBSpHTAAdVR8GbpDdvRvij0k6yrmrjTRVohO5bTaE611KNzXOW5K4Y8PhoTHasNnMEydfAh4ysut92lWObmg2CG1AgMBAAGgADANBgkqhkiG9w0BAQsFAAOBgQCg8dbM4gMxChp4MF67B/0ARv5Ezq3423v/Tkj5KOMxFql+NeYtM9JpIWABMw2xlARl+agp9e8eaj503grhHjYeGV0afC2/8AA2o/PyZOrS80QViDK6Z4cY+zUO5hp3darGCEH14fuAHKwrokSQxYReqdBELyT3r4ZnCdbi+NUx7A==",
                DeviceType = deviceSetupService.DeviceTypeAsString,
            });

            if (certificateResponse == null) ProvisionDeviceFail();

            await Task.Delay(1000);

            SetPairingStatusText("Reconnecting to device to finish up...");

            // reconnect to device to set device type
            var reconnectSuccess = await wifiService.ConnectToWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork.SSID, deviceSetupService.DefaultPassword);

            await Task.Delay(2000);

            if (!reconnectSuccess) ProvisionDeviceFail();

            var provisionSuccess = await onboardingRestService.ProvisionDevice(new RequestCommandProvisionDevice
            {
                DeviceCertificateInformation = new DeviceCertificateModel
                {
                    DeviceType = certificateResponse.DeviceType,
                    Certificate = certificateResponse.Certificate,
                    DpsIdScope = certificateResponse.DpsIdScope,
                    DpsInstance = certificateResponse.DpsInstance,
                },
            });

            if (provisionSuccess == null || !provisionSuccess.IsSuccess) ProvisionDeviceFail();

            var setDeviceTypeResult = await onboardingRestService.SetDeviceType(new RequestCommandSetDeviceType
            {
                DeviceType = certificateResponse.DeviceType,
            });

            OnFinishDevicePairing?.Invoke(this, new OnFinishDevicePairingEventArgs
            {
                IsSuccess = setDeviceTypeResult.IsSuccess,
            });

            if (setDeviceTypeResult.IsSuccess)
            {
                SetPairingStatusText("Pairing Successful!");
            }
            else
            {
                ProvisionDeviceFail();
            }

            // disconnect from device and connect to the internet to provision device with services
            await wifiService.DisconnectFromWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork);

            await Task.Delay(2000);

            return setDeviceTypeResult.IsSuccess;
        }
    }
}
