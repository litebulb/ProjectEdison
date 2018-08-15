using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Foundation.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Devices.Tpm;
using Newtonsoft.Json;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Client;
using Windows.Devices.Gpio;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Edison.SoundSensor
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int PIN_SOUND = 23;

        private ApplicationDataContainer _localSettings;
        private LoggingChannel _logging;
        private TpmDevice _tpmDevice;
        private AzureIoTHubService _azureIoTHubService;
        private SoundSensorConfig _config;
        private bool _isRunning = false;
        private GpioPin _pinSound;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //Deferral task to allow for async
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            //Config
            _localSettings = ApplicationData.Current.LocalSettings;

            //Get TPM Device ConnectionString
            _tpmDevice = new TpmDevice(0);

            //Create services
            _logging = new LoggingChannel("DeviceLED", null, new Guid("8e2fb18a-e7ae-45f9-bf05-d42455ba6ce0"));
            _azureIoTHubService = new AzureIoTHubService(_logging, ReceiveConfiguration);

            await RunApplication();

            deferral.Complete();
        }

        private async Task RunApplication()
        {
            _logging.LogMessage("Initializing Application", LoggingLevel.Verbose);

            //Set up IoT Hub Device
            if (!await _azureIoTHubService.Init(_tpmDevice.GetConnectionString()))
            {
                _logging.LogMessage("The application was not initialized. Please make sure that the TPM service is properly set up on Device 0. Then restart the application.", LoggingLevel.Error);
                return;
            }

            await InitConfig();
            InitGPIO();

            //Run the program
            _isRunning = true;

            do
            {
                await Task.Delay(1000);
            }
            while (_isRunning);

            //Program interrupted, dispose the services
            if(!_azureIoTHubService.Connected)
                _azureIoTHubService.Dispose();
            ClearPins();

            //Restart
            await RunApplication();
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                _logging.LogMessage("There is no GPIO controller on this device.", LoggingLevel.Error);
                return;
            }

            try
            {
                _pinSound = gpio.OpenPin(PIN_SOUND);
                _pinSound.SetDriveMode(GpioPinDriveMode.Input);
                _pinSound.ValueChanged += _pinSound_ValueChanged;
            }
            catch(Exception e)
            {
                _logging.LogMessage(e.Message, LoggingLevel.Error);
            }

            return;
        }

        private void _pinSound_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            _logging.LogMessage(args.Edge == GpioPinEdge.FallingEdge ? "on" : "off", LoggingLevel.Information);
        }

        private void PinTurnOff(GpioPin pin)
        {
            pin.Write(GpioPinValue.High);
        }

        private void PinTurnOn(GpioPin pin)
        {
            pin.Write(GpioPinValue.Low);
        }

        private void ClearPins()
        {
            try
            {
                if (_pinSound != null)
                {
                    PinTurnOff(_pinSound);
                    _pinSound.Dispose();
                    _pinSound = null;
                }
            }
            catch (Exception e)
            {
                _logging.LogMessage(e.Message, LoggingLevel.Error);
            }
        }

        private async Task InitConfig()
        {
            await Task.Delay(1000);

            //TODO
            /*try
            {
                _logging.LogMessage("Initializing Configuration", LoggingLevel.Verbose);
                var desiredProperties = await _azureIoTHubService.GetDeviceTwinAsync();
                _config = JsonConvert.DeserializeObject<SoundSensorConfig>(desiredProperties);
            }
            catch (Exception e)
            {
                _logging.LogMessage($"InitConfig: {e.Message}", LoggingLevel.Critical);
                _config = new SoundSensorConfig();
            }*/
        }

        private async Task ReceiveConfiguration(TwinCollection desiredProperties, object userContext)
        {
            await InitConfig();
        }
    }
}
