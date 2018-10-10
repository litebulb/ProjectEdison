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
using Edison.Devices.Common;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Edison.Devices.SmartBulb
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int PIN_RED = 17;
        private const int PIN_GREEN = 18;
        private const int PIN_BLUE = 27;
        private const int MIN_FREQUENCY_MS = 10;

        // Logging helper
        private LoggingChannel _logging;
        // IoT Hub helper
        private AzureIoTHubService _azureIoTHubService;
        // GPIO helper
        private GPIOService _gpioService;


        // Desired object
        private SmartBulbConfig _config;
        // Keep previous color in check to avoid resetting the pins
        private Color _previousColor;
        // Keep previous pin red
        private int _loadPinRed;
        // Keep previous pin green
        private int _loadPinGreen;
        // Keep previous pin blue
        private int _loadPinBlue;
        // False/True for managing flashing
        private bool _flashingState;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //Deferral task to allow for async
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            var app = new AppIoTBackgroundDeviceTask("Lightbulb", new Guid("26fcf049-de51-4fe4-9015-8a0245fa8aa8"));
            app.SetInitApplication(InitApplication);
            app.SetStartApplication(StartApplication);
            app.SetRunApplicationLoop(RunApplication);
            app.SetChangeConfiguration(ReceiveDesiredConfiguration);

            await app.Run();
            deferral.Complete();
        }

        /// <summary>
        /// Initial application - Done once
        /// </summary>
        /// <returns></returns>
        private bool InitApplication(LoggingChannel logging, AzureIoTHubService azureIoTService, GPIOService gpioService)
        {
            //Config
            //var localSettings = ApplicationData.Current.LocalSettings;
            _logging = logging;
            _azureIoTHubService = azureIoTService;
            _gpioService = gpioService;
            return true;
        }

        /// <summary>
        /// Code running when the application starts or restarts
        /// </summary>
        /// <returns></returns>
        private async Task StartApplication()
        {
            _previousColor = Color.Unknown;
            _loadPinGreen = 0;
            _loadPinRed = 0;
            _loadPinBlue = 0;
            await LoadApplicationConfig();
        }

        /// <summary>
        /// Code running when the application starts or restarts
        /// </summary>
        /// <returns></returns>
        private async Task LoadApplicationConfig()
        {
            _logging.LogMessage("Retrieve desired properties", LoggingLevel.Verbose);
            var desiredProperties = await _azureIoTHubService.GetDeviceTwinAsync();
            _config = JsonConvert.DeserializeObject<SmartBulbConfig>(desiredProperties);

            if (_config != null)
            {
                if (_config.GpioConfig == null)
                {
                    _config.GpioConfig = new SmartBulbGpioConfig()
                    {
                        GpioColorRed = PIN_RED,
                        GpioColorGreen = PIN_GREEN,
                        GpioColorBlue = PIN_BLUE
                    };
                }

                _config.FlashFrequency = _config.FlashFrequency < MIN_FREQUENCY_MS ? MIN_FREQUENCY_MS : _config.FlashFrequency;

                if (_config.State == State.Flashing || _config.State == State.On)
                {
                    if (_config.GpioConfig.GpioColorRed != _loadPinRed)
                    {
                        _loadPinRed = _config.GpioConfig.GpioColorRed;
                        _gpioService.InitGPIOOutput(_loadPinRed);
                    }
                    if (_config.GpioConfig.GpioColorGreen != _loadPinGreen)
                    {
                        _loadPinGreen = _config.GpioConfig.GpioColorGreen;
                        _gpioService.InitGPIOOutput(_loadPinGreen);
                    }
                    if (_config.GpioConfig.GpioColorBlue != _loadPinBlue)
                    {
                        _loadPinBlue = _config.GpioConfig.GpioColorBlue;
                        _gpioService.InitGPIOOutput(_loadPinBlue);
                    }
                    _previousColor = Color.Unknown;
                }
            }
        }

        private async Task RunApplication()
        {
            //Flashing behavior
            if (_config.State == State.Flashing)
            {
                _flashingState = !_flashingState;
                if (_flashingState)
                    SetLED(_config.Color);
                else
                    SetLED(Color.Off);
                await Task.Delay(_config.FlashFrequency);
            }
            //Continous light behavior
            else if(_config.State == State.On)
            {
                if (_config.Color != _previousColor)
                {
                    SetLED(_config.Color);
                    _previousColor = _config.Color;
                }
                await Task.Delay(500);
            }
        }

        private void SetLED(Color color)
        {
            switch (color)
            {
                case Color.White:
                    SetLED(true, true, true);
                    break;
                case Color.Red:
                    SetLED(true, false, false);
                    break;
                case Color.Green:
                    SetLED(false, true, false);
                    break;
                case Color.Blue:
                    SetLED(false, false, true);
                    break;
                case Color.Yellow:
                    SetLED(true, true, false);
                    break;
                case Color.Purple:
                    SetLED(true, false, true);
                    break;
                case Color.Cyan:
                    SetLED(false, true, true);
                    break;
                case Color.Off:
                    SetLED(false, false, false);
                    break;
            }
        }

        private void SetLED(bool red, bool green, bool blue)
        {
            if (red)
                _gpioService.PinSetLow(_config.GpioConfig.GpioColorRed);
            else
                _gpioService.PinSetHigh(_config.GpioConfig.GpioColorRed);

            if (green)
                _gpioService.PinSetLow(_config.GpioConfig.GpioColorGreen);
            else
                _gpioService.PinSetHigh(_config.GpioConfig.GpioColorGreen);

            if (blue)
                _gpioService.PinSetLow(_config.GpioConfig.GpioColorBlue);
            else
                _gpioService.PinSetHigh(_config.GpioConfig.GpioColorBlue);
        }

        private async Task ReceiveDesiredConfiguration(TwinCollection desiredProperties)
        {
            await LoadApplicationConfig();
        }
    }
}
