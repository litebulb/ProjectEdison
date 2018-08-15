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

namespace Edison.SmartBulb
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int PIN_RED = 17;
        private const int PIN_GREEN = 18;
        private const int PIN_BLUE = 27;

        private ApplicationDataContainer _localSettings;
        private LoggingChannel _logging;
        private TpmDevice _tpmDevice;
        private AzureIoTHubService _azureIoTHubService;
        private SmartBulbConfig _config;
        private bool _isRunning = false;
        private readonly string[] _allowedColors = new string[] { "white", "red", "green", "blue", "yellow", "purple", "cyan", "off" };
        private GpioPin _pinRed;
        private GpioPin _pinGreen;
        private GpioPin _pinBlue;
        private string _previousColor;

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
                ConfigureColor();
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
                _pinRed = gpio.OpenPin(PIN_RED);
                _pinRed.SetDriveMode(GpioPinDriveMode.Output);
                PinTurnOff(_pinRed);

                _pinGreen = gpio.OpenPin(PIN_GREEN);
                _pinGreen.SetDriveMode(GpioPinDriveMode.Output);
                PinTurnOff(_pinGreen);

                _pinBlue = gpio.OpenPin(PIN_BLUE);
                _pinBlue.SetDriveMode(GpioPinDriveMode.Output);
                PinTurnOff(_pinBlue);
            }
            catch(Exception e)
            {
                _logging.LogMessage(e.Message, LoggingLevel.Error);
            }

            return;
        }

        private void PinTurnOff(GpioPin pin)
        {
            pin.Write(GpioPinValue.High);
            //pin.SetDriveMode(GpioPinDriveMode.Input);
        }

        private void PinTurnOn(GpioPin pin)
        {
            //pin.SetDriveMode(GpioPinDriveMode.Output);
            pin.Write(GpioPinValue.Low);
        }

        private void ConfigureColor()
        {
            if (_config.Color == _previousColor)
                return;

            switch (_config.Color)
            {
                case "white":
                    PinTurnOn(_pinRed);
                    PinTurnOn(_pinGreen);
                    PinTurnOn(_pinBlue);
                    break;
                case "red":
                    PinTurnOff(_pinGreen);
                    PinTurnOff(_pinBlue);
                    PinTurnOn(_pinRed);
                    break;
                case "green":
                    PinTurnOff(_pinRed);
                    PinTurnOff(_pinBlue);
                    PinTurnOn(_pinGreen);
                    break;
                case "blue":
                    PinTurnOff(_pinRed);
                    PinTurnOff(_pinGreen);
                    PinTurnOn(_pinBlue);
                    break;
                case "yellow":
                    PinTurnOff(_pinBlue);
                    PinTurnOn(_pinRed);
                    PinTurnOn(_pinGreen);
                    break;
                case "purple":
                    PinTurnOff(_pinGreen);
                    PinTurnOn(_pinRed);
                    PinTurnOn(_pinBlue);
                    break;
                case "cyan":
                    PinTurnOff(_pinRed);
                    PinTurnOn(_pinGreen);
                    PinTurnOn(_pinBlue);
                    break;
                case "off":
                    PinTurnOff(_pinRed);
                    PinTurnOff(_pinGreen);
                    PinTurnOff(_pinBlue);
                    break;
            }

            _previousColor = _config.Color;
        }

        private void ClearPins()
        {
            try
            {
                if (_pinRed != null)
                {
                    PinTurnOff(_pinRed);
                    _pinRed.Dispose();
                    _pinRed = null;
                }
                if (_pinGreen != null)
                {
                    PinTurnOff(_pinGreen);
                    _pinGreen.Dispose();
                    _pinGreen = null;
                }
                if (_pinBlue != null)
                {
                    PinTurnOff(_pinBlue);
                    _pinBlue.Dispose();
                    _pinBlue = null;
                }
            }
            catch (Exception e)
            {
                _logging.LogMessage(e.Message, LoggingLevel.Error);
            }
        }

        private async Task InitConfig()
        {
            try
            {
                _logging.LogMessage("Initializing Configuration", LoggingLevel.Verbose);
                var desiredProperties = await _azureIoTHubService.GetDeviceTwinAsync();
                _config = JsonConvert.DeserializeObject<SmartBulbConfig>(desiredProperties);
            }
            catch (Exception e)
            {
                _logging.LogMessage($"InitConfig: {e.Message}", LoggingLevel.Critical);
                _config = new SmartBulbConfig();
            }

            _config.Color = _config.Color.ToLower();
            if (string.IsNullOrEmpty(_config.Color) || _allowedColors.FirstOrDefault(p => p == _config.Color) == null)
                _config.Color = "white";
        }

        private async Task ReceiveConfiguration(TwinCollection desiredProperties, object userContext)
        {
            await InitConfig();
        }
    }
}
