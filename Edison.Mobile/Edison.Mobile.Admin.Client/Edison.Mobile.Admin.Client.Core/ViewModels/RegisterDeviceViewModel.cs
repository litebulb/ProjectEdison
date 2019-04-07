using System;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.Admin.Client.Core.Models;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Admin.Client.Core.Shared;
using Edison.Mobile.Common.Shared;
using Edison.Mobile.Common.WiFi;
using Edison.Mobile.Admin.Client.Core.Ioc;
using System.Threading;

namespace Edison.Mobile.Admin.Client.Core.ViewModels
{
    public class RegisterDeviceViewModel : DeviceSetupBaseViewModel
    {

        public enum RegistrationState
        {
            New,
            Starting,
            ConnectingConnectingToDeviceFirstTime,
            ConnectedToDevice,
            DeviceInfoGenerated,
            ProvisioningWithCloud,
            ConnectingConnectingToDeviceSecondTime
        }

        readonly IDeviceRestService deviceRestService;


        public RegistrationState State { get; private set; }
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
            IDeviceRestService deviceRestService,
            IWifiService wifiService,
            IOnboardingRestService onboardingRestService,
            DeviceProvisioningRestService deviceProvisioningRestService
        ) : base(deviceSetupService, deviceProvisioningRestService, onboardingRestService, wifiService)
        {
            this.deviceRestService = deviceRestService;
        }

        public override void BindEventHandlers()
        {
            base.BindEventHandlers();
            this.wifiService.ConnectionFailed += WifiService_ConnectionFailed;
            this.wifiService.CheckingConnectionStatusUpdated += WifiService_CheckingConnectionStatusUpdated;
        }

        public override void UnBindEventHandlers()
        {
            base.UnBindEventHandlers();
            this.wifiService.ConnectionFailed -= WifiService_ConnectionFailed;
            this.wifiService.CheckingConnectionStatusUpdated -= WifiService_CheckingConnectionStatusUpdated;
            State = RegistrationState.New;
        }

        private void WifiService_ConnectionFailed(object sender, Common.WiFi.ConnectionFailedEventArgs e)
        {
            OnFinishDevicePairing?.Invoke(this, new OnFinishDevicePairingEventArgs() { IsSuccess = false });            
        }
        
        async Task<bool> ProvisionDeviceFail()
        {
            await wifiService.DisconnectFromWifiNetwork(deviceSetupService.CurrentDeviceHotspotNetwork);

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

        private void WifiService_CheckingConnectionStatusUpdated(object sender, Common.WiFi.CheckingConnectionStatusUpdatedEventArgs e)
        {
            if (e.IsConnected)
            {
                if (DeviceSetupService.SSIDIsEdisonDevice(e.SSID))
                {
                    switch (State)
                    {
                        case RegistrationState.ConnectingConnectingToDeviceFirstTime:                        
                            State = RegistrationState.ConnectedToDevice;
                            Task.Factory.StartNew(async () =>
                            {
                                await GenerateDeviceInfo(new WifiNetwork() { SSID = e.SSID });
                            }, new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token);
                            break;
                        case RegistrationState.ConnectingConnectingToDeviceSecondTime:
                            State = RegistrationState.ConnectedToDevice;
                            Task.Factory.StartNew(async () =>
                            {
                                await OnboardDevice();
                            }, new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);
                            break;
                        default:
                            SetPairingStatusText($"State was set to {State}");
                            break;
                    }

                }
                else
                {
                    if (State == RegistrationState.DeviceInfoGenerated)
                    {
                        State = RegistrationState.ProvisioningWithCloud;
                        Task.Factory.StartNew(async () =>
                        {
                            await ProvisionWithCloud();
                        }, new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token);
                    }
                }
            }

        }

        private async Task<bool> GenerateDeviceInfo(WifiNetwork wifiNetwork)
        {
            // get stuff from device
            SetPairingStatusText("Grabbing some information from the device...");
            
            onboardingRestService.SetBasicAuthentication(deviceSetupService.DefaultPortalPassword);

            var deviceIdResponse = await onboardingRestService.GetDeviceId();

            if (deviceIdResponse == null) return await ProvisionDeviceFail();

            deviceSetupService.CurrentDeviceModel.DeviceId = deviceIdResponse.DeviceId;

            //return deviceIdResponse?.DeviceId; // shortcut for testing

            csrResult = await onboardingRestService.GetGeneratedCSR();

            State = RegistrationState.DeviceInfoGenerated;
            var success = await wifiService.ConnectToWifiNetwork(deviceSetupService.OriginalSSID);

            return true;
        }

        ResultCommandGenerateCSR csrResult = default(ResultCommandGenerateCSR);
        DeviceCertificateModel certificateResponse = default(DeviceCertificateModel);
        DeviceSecretKeysModel generateKeysResponse = default(DeviceSecretKeysModel);

        private async Task<bool> ProvisionWithCloud()
        {

            if (csrResult == null) return await ProvisionDeviceFail();

            // provision device with azure
            SetPairingStatusText("Provisioning device with the mothership...");

            certificateResponse = await deviceProvisioningRestService.GenerateDeviceCertificate(new DeviceCertificateRequestModel
            {
                Csr = csrResult?.Csr ?? "MIIBbjCB2AIBADAvMS0wKwYDVQQDEyQ4OTJlYWM5YS1iOWFkLTQ0NDgtYWEwYS0wOTI0MDE1YWMwMWEwgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGBALeqOH+XoeXXERg8neKzr3IumxTDMKsPzKjZ/kfE1gu/FHmr1ugPuRTtQzP5WFVD5lWqtEKJyX+YDCjNevKeHBSpHTAAdVR8GbpDdvRvij0k6yrmrjTRVohO5bTaE611KNzXOW5K4Y8PhoTHasNnMEydfAh4ysut92lWObmg2CG1AgMBAAGgADANBgkqhkiG9w0BAQsFAAOBgQCg8dbM4gMxChp4MF67B/0ARv5Ezq3423v/Tkj5KOMxFql+NeYtM9JpIWABMw2xlARl+agp9e8eaj503grhHjYeGV0afC2/8AA2o/PyZOrS80QViDK6Z4cY+zUO5hp3darGCEH14fuAHKwrokSQxYReqdBELyT3r4ZnCdbi+NUx7A==",
                DeviceType = deviceSetupService.DeviceTypeAsString,
            });

            if (certificateResponse == null) return await ProvisionDeviceFail();

            generateKeysResponse = await deviceProvisioningRestService.GenerateDeviceKeys(deviceSetupService.CurrentDeviceModel.DeviceId, deviceSetupService.CurrentDeviceModel.SSID);

            if (generateKeysResponse == null) return await ProvisionDeviceFail();

            await Task.Delay(2000);

            SetPairingStatusText("Reconnecting to device and finishing up! Sit tight...");

            State = RegistrationState.ConnectingConnectingToDeviceSecondTime;
            // reconnect to device to set device type
            var reconnectSuccess = await wifiService.ConnectToWifiNetwork(deviceSetupService.CurrentDeviceModel.SSID, deviceSetupService.DefaultPassword);

            if (!reconnectSuccess) return await ProvisionDeviceFail();
            return true;
        }

        private async Task<bool> OnboardDevice()
        {

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

            SetPairingStatusText("Updating secrets on the device! Sit tight...");

            if (provisionSuccess == null || !provisionSuccess.IsSuccess) return await ProvisionDeviceFail();

            var setDeviceKeysResponse = await onboardingRestService.SetDeviceSecretKeys(new RequestCommandSetDeviceSecretKeys
            {
                AccessPointPassword = generateKeysResponse.SSIDPassword,
                EncryptionKey = generateKeysResponse.EncryptionKey,
                PortalPassword = generateKeysResponse.PortalPassword,
            });

            if (setDeviceKeysResponse == null || !setDeviceKeysResponse.IsSuccess) return await ProvisionDeviceFail();

            deviceSetupService.PortalPassword = generateKeysResponse.PortalPassword;
            deviceSetupService.WiFiPassword = generateKeysResponse.SSIDPassword;
  
            onboardingRestService.SetBasicAuthentication(deviceSetupService.PortalPassword);

            SetPairingStatusText("Setting the Device Type");
            var setDeviceTypeResult = await onboardingRestService.SetDeviceType(new RequestCommandSetDeviceType
            {
                DeviceType = certificateResponse.DeviceType,
            });

            if (setDeviceTypeResult == null || !setDeviceTypeResult.IsSuccess) return await ProvisionDeviceFail();

            OnFinishDevicePairing?.Invoke(this, new OnFinishDevicePairingEventArgs
            {
                IsSuccess = setDeviceTypeResult.IsSuccess,
            });

            SetPairingStatusText("Pairing Successful!");

            deviceSetupService.CurrentDeviceModel.DeviceType = certificateResponse.DeviceType;

            // stay connected to device to get available wifi networks on the next screen....

            return true;
        }

        public async Task<bool> ProvisionDevice(WifiNetwork wifiNetwork)
        {
            State = RegistrationState.Starting;

            OnBeginDevicePairing?.Invoke();
            this.deviceSetupService.CurrentDeviceModel.SSID = wifiNetwork.SSID;
            this.deviceSetupService.CurrentDeviceHotspotNetwork = new WifiNetwork() { SSID = wifiNetwork.SSID };

            // connect to device
            SetPairingStatusText("Connecting to device...");

            var defaultWifiNetwork = await wifiService.GetCurrentlyConnectedWifiNetwork();
            deviceSetupService.OriginalSSID = defaultWifiNetwork.SSID;
                       
            State = RegistrationState.ConnectingConnectingToDeviceFirstTime;
            var success = await wifiService.ConnectToWifiNetwork(wifiNetwork.SSID, deviceSetupService.DefaultPassword);

            if (!success) return await ProvisionDeviceFail();
                       
            return true;
        }

        void SetPairingStatusText(string statusText)
        {
            OnPairingStatusTextChanged?.Invoke(this, statusText);
        }
    }
}
