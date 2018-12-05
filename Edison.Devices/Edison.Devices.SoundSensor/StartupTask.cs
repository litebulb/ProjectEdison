using System;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Edison.Devices.Common;
using Edison.Devices.SoundSensor.Messages;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Edison.Devices.SoundSensor
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int PIN_SOUND = 23;
        private const int SOUND_TRIGGER_DELAY_MINIMUM = 2000;

        // Logging helper
        private LoggingChannel _logging;
        // IoT Hub helper
        private AzureIoTHubService _azureIoTHubService;
        // GPIO helper
        private GPIOService _gpioService;
        // Desired object
        private SoundSensorState _config;
        // Randomizer for decibels
        private Random _rand = new Random();
        // Value where next sound trigger can happen
        private DateTime _nextTrigger = DateTime.MinValue;
        //Type event
        private readonly string _eventType = "sound";


        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //Deferral task to allow for async
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            var app = new AppIoTBackgroundDeviceTask<SoundSensorState,SoundSensorStateReported>("SoundSensor", new Guid("8e2fb18a-e7ae-45f9-bf05-d42455ba6ce0"));
            app.InitApplication += InitApplication;
            app.StartApplication += StartApplication;
            app.ChangeConfiguration += ReceiveDesiredConfiguration;

            await app.Run();
            deferral.Complete();
        }

        #pragma warning disable 1998
        private async Task<SoundSensorStateReported> ReceiveDesiredConfiguration(SoundSensorState desiredProperties)
        {
            _config = desiredProperties;
            //Here the behavior to handle decibel threshold would be added
            //---
            return new SoundSensorStateReported() { DeviceState = _config };
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

        #pragma warning disable 1998
        /// <summary>
        /// Code running when the application starts or restarts
        /// </summary>
        /// <returns></returns>
        private async Task<SoundSensorStateReported> StartApplication(SoundSensorState desiredProperties)
        {
            _config = desiredProperties;
            
            if (_config != null)
            {
                if (_config.GpioConfig == null)
                {
                    _config.GpioConfig = new SoundSensorGpioConfig() { GpioSound = PIN_SOUND };
                }

                if (_config.SoundTriggerDelay < SOUND_TRIGGER_DELAY_MINIMUM)
                    _config.SoundTriggerDelay = SOUND_TRIGGER_DELAY_MINIMUM;

                if (_config.State == DeviceState.On)
                    _gpioService.InitGPIOInput(_config.GpioConfig.GpioSound, GpioSoundChanged);
            }

            return new SoundSensorStateReported() { DeviceState = _config };
        }


        #pragma warning disable 4014
        private void GpioSoundChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.FallingEdge && _nextTrigger < DateTime.UtcNow)
            {
                _azureIoTHubService.SendIoTMessage(new SoundSensorMessage()
                {
                    Decibel = (_rand.NextDouble() * 20.00) + 120.00
                }, _eventType);
                _nextTrigger = DateTime.UtcNow.AddMilliseconds(_config.SoundTriggerDelay);
            }
        }
    }
}
