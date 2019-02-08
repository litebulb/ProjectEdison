using System;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.Models;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Admin.Client.Core.Shared;
using Edison.Mobile.Common.Shared;
using Edison.Mobile.Common.ViewModels;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class RegisterDeviceViewModel : DeviceSetupBaseViewModel
    {
        readonly IWifiService wifiService;
        readonly DeviceRestService deviceRestService;
        readonly OnboardingRestService onboardingRestService;
        readonly DeviceProvisioningRestService deviceProvisioningRestService;

        public event ViewNotification OnBeginDevicePairing;
        public event EventHandler<OnFinishDevicePairingEventArgs> OnFinishDevicePairing;
        public event EventHandler<string> OnPairingStatusTextChanged;

        public class OnFinishDevicePairingEventArgs : EventArgs
        {
            public bool IsSuccess { get; set; }
        }

        public string MockDeviceId => "BA27EB910E94";

        public RegisterDeviceViewModel(
            DeviceSetupService deviceSetupService, 
            DeviceRestService deviceRestService,
            IWifiService wifiService,
            OnboardingRestService onboardingRestService,
            DeviceProvisioningRestService deviceProvisioningRestService
        ) : base(deviceSetupService)
        {
            this.deviceRestService = deviceRestService;
            this.wifiService = wifiService;
            this.onboardingRestService = onboardingRestService;
            this.deviceProvisioningRestService = deviceProvisioningRestService;
        }

        async Task<bool> ProvisionDeviceFail()
        {
            await wifiService.DisconnectFromWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork);

            await Task.Delay(2000);

            OnFinishDevicePairing?.Invoke(this, new OnFinishDevicePairingEventArgs
            {
                IsSuccess = false,
            });

            return false;
        }

        public async Task<DeviceModel> GetDevice(Guid deviceId)
        {
            return await deviceRestService.GetDevice(deviceId);
        }

        public async Task<bool> ProvisionDevice(WifiNetwork wifiNetwork)
        {

            Guid.TryParse("ed655b50-a146-47e4-a1d7-4cd2d940eedb", out var guid);
            var deviceModel = await GetDevice(guid);

            OnBeginDevicePairing?.Invoke();

            // connect to device
            SetPairingStatusText("Connecting to device...");

            var defaultWifiNetwork = await wifiService.GetCurrentlyConnectedWifiNetwork();
            var success = await wifiService.ConnectToWifiNetwork(wifiNetwork.SSID, deviceSetupService.DefaultPassword);

            await Task.Delay(2000);

            if (!success) return await ProvisionDeviceFail();


            // get stuff from device
            SetPairingStatusText("Grabbing some information from the device...");

            deviceSetupService.CurrentDeviceHotspotNetwork = success ? wifiNetwork : null;

            var deviceIdResponse = await onboardingRestService.GetDeviceId();

            if (deviceIdResponse == null) return await ProvisionDeviceFail();

            deviceSetupService.CurrentDeviceModel.DeviceId = deviceIdResponse.DeviceId;

            //return deviceIdResponse?.DeviceId; // shortcut for testing

            var csrResult = await onboardingRestService.GetGeneratedCSR();

            // disconnect from device and connect to the internet to provision device with services
            await wifiService.DisconnectFromWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork);

            await Task.Delay(2000);

            if (csrResult == null) return await ProvisionDeviceFail();

            // provision device with azure
            SetPairingStatusText("Provisioning device with the mothership...");

            var certificateResponse = await deviceProvisioningRestService.GenerateDeviceCertificate(new DeviceCertificateRequestModel
            {
                Csr = csrResult?.Csr ?? "MIIBbjCB2AIBADAvMS0wKwYDVQQDEyQ4OTJlYWM5YS1iOWFkLTQ0NDgtYWEwYS0wOTI0MDE1YWMwMWEwgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGBALeqOH+XoeXXERg8neKzr3IumxTDMKsPzKjZ/kfE1gu/FHmr1ugPuRTtQzP5WFVD5lWqtEKJyX+YDCjNevKeHBSpHTAAdVR8GbpDdvRvij0k6yrmrjTRVohO5bTaE611KNzXOW5K4Y8PhoTHasNnMEydfAh4ysut92lWObmg2CG1AgMBAAGgADANBgkqhkiG9w0BAQsFAAOBgQCg8dbM4gMxChp4MF67B/0ARv5Ezq3423v/Tkj5KOMxFql+NeYtM9JpIWABMw2xlARl+agp9e8eaj503grhHjYeGV0afC2/8AA2o/PyZOrS80QViDK6Z4cY+zUO5hp3darGCEH14fuAHKwrokSQxYReqdBELyT3r4ZnCdbi+NUx7A==",
                DeviceType = deviceSetupService.DeviceTypeAsString,
            });

            if (certificateResponse == null) return await ProvisionDeviceFail();

            await Task.Delay(2000);

            SetPairingStatusText("Reconnecting to device and finishing up! Sit tight...");

            // reconnect to device to set device type
            var reconnectSuccess = await wifiService.ConnectToWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork.SSID, deviceSetupService.DefaultPassword);

            await Task.Delay(2000);

            if (!reconnectSuccess) return await ProvisionDeviceFail();

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

            if (provisionSuccess == null || !provisionSuccess.IsSuccess) return await ProvisionDeviceFail();

            var setDeviceTypeResult = await onboardingRestService.SetDeviceType(new RequestCommandSetDeviceType
            {
                DeviceType = certificateResponse.DeviceType,
            });

            if (setDeviceTypeResult == null) return await ProvisionDeviceFail(); 

            OnFinishDevicePairing?.Invoke(this, new OnFinishDevicePairingEventArgs
            {
                IsSuccess = setDeviceTypeResult.IsSuccess,
            });

            if (!setDeviceTypeResult.IsSuccess) return await ProvisionDeviceFail();

            SetPairingStatusText("Pairing Successful!");

            deviceSetupService.CurrentDeviceModel.DeviceType = certificateResponse.DeviceType;

            // stay connected to device to get available wifi networks on the next screen....

            return true;
        }

        void SetPairingStatusText(string statusText)
        {
            OnPairingStatusTextChanged?.Invoke(this, statusText);
        }
    }
}
