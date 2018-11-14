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
using Edison.Devices.SoundSensor.Messages;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Edison.Devices.SoundSensor
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int PIN_SOUND = 23;

        // Logging helper
        private LoggingChannel _logging;
        // IoT Hub helper
        private AzureIoTHubService _azureIoTHubService;
        // GPIO helper
        private GPIOService _gpioService;


        // If the sound sensor is triggered more than once at a time, this value ensure that no more than one message can be sent every 2 seconds.
        private readonly int _soundTriggerDelay = 2000;
        // Desired object
        private SoundSensorConfig _config;
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

            var app = new AppIoTBackgroundDeviceTask("SoundSensor", new Guid("8e2fb18a-e7ae-45f9-bf05-d42455ba6ce0"));
            app.InitApplication += InitApplication;
            app.StartApplication += StartApplication;

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
            _logging.LogMessage("Retrieve desired properties", LoggingLevel.Verbose);
            var desiredProperties = await _azureIoTHubService.GetDeviceTwinAsync();
            if (string.IsNullOrEmpty(desiredProperties))
            {
                _logging.LogMessage("Cannot retrieve desired properties", LoggingLevel.Error);
                return;
            }

            _config = JsonConvert.DeserializeObject<SoundSensorConfig>(desiredProperties);

            if (_config != null)
            {
                if (_config.GpioConfig == null)
                {
                    _config.GpioConfig = new SoundSensorGpioConfig() { GpioSound = PIN_SOUND };
                }

                if(_config.State == State.On)
                    _gpioService.InitGPIOInput(_config.GpioConfig.GpioSound, GpioSoundChanged);
            }
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
                _nextTrigger = DateTime.UtcNow.AddMilliseconds(_soundTriggerDelay);
            }
        }
    }
}
