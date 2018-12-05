using System;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Diagnostics;
using System.Threading.Tasks;
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
        private SmartBulbState _config;
        // Keep previous color in check to avoid resetting the pins
        private ColorState _previousColor;
        // Keep previous pin red
        private int _loadPinRed;
        // Keep previous pin green
        private int _loadPinGreen;
        // Keep previous pin blue
        private int _loadPinBlue;
        // False/True for managing flashing
        private bool _flashingState;
        private bool _disconnected = true;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //Deferral task to allow for async
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            var app = new AppIoTBackgroundDeviceTask<SmartBulbState,SmartBulbStateReported>("SmartBulb", new Guid("26fcf049-de51-4fe4-9015-8a0245fa8aa8"));
            app.InitApplication += InitApplication;
            app.StartApplication += StartApplication;
            app.RunApplication += RunApplication;
            app.EndApplication += EndApplication;
            app.GeneralError += GeneralError;
            app.ChangeConfiguration += ReceiveDesiredConfiguration;
            app.DisconnectedApplication += DisconnectedApplication;
            app.TestMethod += TestMethod;

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
        private async Task<SmartBulbStateReported> StartApplication(SmartBulbState desiredProperties)
        {
            _config = desiredProperties;
            await ConfigureApplicationConfig();
            return new SmartBulbStateReported() { DeviceState = _config };
        }

        #pragma warning disable 1998
        /// <summary>
        /// Code running when the application ends
        /// </summary>
        /// <returns></returns>
        private async Task EndApplication()
        {
            _previousColor = ColorState.Unknown;
            _loadPinGreen = 0;
            _loadPinRed = 0;
            _loadPinBlue = 0;
            _disconnected = true;
        }


        private async Task<SmartBulbStateReported> ReceiveDesiredConfiguration(SmartBulbState desiredProperties)
        {
            _config = desiredProperties;
            await ConfigureApplicationConfig();
            return new SmartBulbStateReported() { DeviceState = _config };
        }

        private async Task ConfigureApplicationConfig()
        {
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

                if (_config.State == DeviceState.On)
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
                    _previousColor = ColorState.Unknown;
                }

                if (_disconnected && !_config.IgnoreFlashAlerts)
                {
                    await BlinkLED(ColorState.Green, 2, 200);
                    if (_previousColor != ColorState.Unknown)
                        SetLED(_previousColor);
                    _disconnected = false;
                }
            }
        }

        private async Task DisconnectedApplication()
        {
            if (_config == null || !_config.IgnoreFlashAlerts)
            {
                await BlinkLED(ColorState.Red, 2, 200);
                SetLED(_previousColor);
                _disconnected = true;
            }
        }

        private async Task GeneralError()
        {
            if (_config == null || !_config.IgnoreFlashAlerts)
            {
                await BlinkLED(ColorState.Purple, 2, 100);
                SetLED(_previousColor);
                _disconnected = true;
            }
        }

        private async Task BlinkLED(ColorState color, int durationSeconds, int periodMs)
        {
            int loops = durationSeconds * (1000 / periodMs);
            int periodDivided = periodMs / 2;
            for (int i = 0; i < loops; i++)
            {
                SetLED(color);
                await Task.Delay(periodDivided);
                SetLED(ColorState.Off);
                await Task.Delay(periodDivided);
            }
        }

        private async Task RunApplication()
        {
            //Flashing behavior
            if (_config.LightState == LightState.Flashing)
            {
                _flashingState = !_flashingState;
                if (_flashingState)
                    SetLED(_config.Color);
                else
                    SetLED(ColorState.Off);
                await Task.Delay(_config.FlashFrequency);
            }
            //Continous light behavior
            else if(_config.LightState == LightState.Continuous)
            {
                if (_config.Color != _previousColor)
                {
                    SetLED(_config.Color);
                    _previousColor = _config.Color;
                }
                await Task.Delay(500);
            }
        }

        private void SetLED(ColorState color)
        {
            switch (color)
            {
                case ColorState.White:
                    SetLED(true, true, true);
                    break;
                case ColorState.Red:
                    SetLED(true, false, false);
                    break;
                case ColorState.Green:
                    SetLED(false, true, false);
                    break;
                case ColorState.Blue:
                    SetLED(false, false, true);
                    break;
                case ColorState.Yellow:
                    SetLED(true, true, false);
                    break;
                case ColorState.Purple:
                    SetLED(true, false, true);
                    break;
                case ColorState.Cyan:
                    SetLED(false, true, true);
                    break;
                case ColorState.Off:
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

        private async Task TestMethod()
        {
            await BlinkLED(ColorState.White, 20, 500);
            SetLED(_previousColor);
        }
    }
}
