using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

namespace Edison.Devices.Common
{
    #pragma warning disable 1998
    public class AppIoTBackgroundDeviceTask<D,R> 
        where D : IDeviceState 
        where R : IDeviceStateReported
    {
        /// <summary>
        /// Next time that the device need to ping IoT Hub
        /// </summary>
        private DateTime _nextPingTime = DateTime.MinValue;
        /// <summary>
        /// Switch to interrupt and restart the application
        /// </summary>
        private bool _interruptApplication;
        /// <summary>
        /// Switch to confirm that the configuration was loaded once
        /// </summary>
        private bool _configurationLoaded;
        /// <summary>
        /// Logging helper
        /// </summary>
        private LoggingChannel _logging;
        /// <summary>
        /// IoT Hub helper
        /// </summary>
        private AzureIoTHubService _azureIoTHubService;
        /// <summary>
        /// GPIO helper
        /// </summary>
        private GPIOService _gpioService;
        /// <summary>
        /// Id of the application
        /// </summary>
        private readonly Guid _loggingAppId = Guid.Empty;
        /// <summary>
        /// Name of the application
        /// </summary>
        private readonly string _loggingAppName = "IoTApp";
        /// <summary>
        /// Ping interval to IoT Hub
        /// </summary>
        private readonly int _pingIntervalSecond = 60;
        /// <summary>
        /// Event type for test direct method
        /// </summary>
        private readonly string _eventTypeTest = "test";
        /// <summary>
        /// Hook for Init Application 
        /// </summary>
        public event Func<LoggingChannel, AzureIoTHubService, GPIOService, bool> InitApplication;
        /// <summary>
        /// Hook for Start Application 
        /// </summary>
        public event Func<D, Task<R>> StartApplication;
        /// <summary>
        /// Hook for Start Application 
        /// </summary>
        public event Func<Task> TestMethod;
        /// <summary>
        /// Hook for Run Application loop
        /// </summary>
        public event Func<Task> RunApplication;
        /// <summary>
        /// Hook for End Application 
        /// </summary>
        public event Func<Task> EndApplication;
        /// <summary>
        /// Hook for Disconnected 
        /// </summary>
        public event Func<Task> DisconnectedApplication;
        /// <summary>
        /// Hook for General Error in loop 
        /// </summary>
        public event Func<Task> GeneralError;
        /// <summary>
        /// Hook for Change Configuration 
        /// </summary>
        public event Func<D, Task<R>> ChangeConfiguration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appId"></param>
        public AppIoTBackgroundDeviceTask(string appName, Guid appId)
        {
            _loggingAppName = appName;
            _loggingAppId = appId;
        }

        /// <summary>
        /// IBackgroundTask starting point. Initializes logging
        /// </summary>
        /// <param name="taskInstance">IBackgroundTaskInstance object</param>
        public async Task Run()
        {
            //Create services
            _logging = new LoggingChannel(_loggingAppName, null, _loggingAppId);
            _azureIoTHubService = new AzureIoTHubService(_loggingAppId);
            _azureIoTHubService.SetDesiredPropertyCallback(ReceiveDesiredConfiguration);
            _gpioService = new GPIOService(_loggingAppId);

            //First time configuration (runs only once)
            _logging.LogMessage("Initializing Application", LoggingLevel.Verbose);
            if (!InitApplicationInternal())
                return;

            //Run application (loop indefinitely)
            _logging.LogMessage("Run Application", LoggingLevel.Verbose);
            await RunApplicationInternal();
        }

        private async Task RunApplicationInternal()
        {
            //Application phase
            try
            {
                //Application Loop

                _logging.LogMessage("Entering Application Loop", LoggingLevel.Verbose);
                _interruptApplication = false;
                while (!_interruptApplication)
                {
                    _logging.LogMessage("Connecting to IoT Hub", LoggingLevel.Verbose);
                    //Ensure connection
                    if (await _azureIoTHubService.Init())
                    {
                        //Start application hook
                        _logging.LogMessage("Starting Application", LoggingLevel.Verbose);
                        await StartApplicationInternal();

                        //Main Loop
                        _logging.LogMessage("Entering Logic Loop", LoggingLevel.Verbose);
                        while (!_interruptApplication && _azureIoTHubService.Connected)
                        {
                            if (_nextPingTime < DateTime.UtcNow)
                            {
                                await _azureIoTHubService.SendPingMessage();
                                _nextPingTime = DateTime.UtcNow.AddSeconds(_pingIntervalSecond);
                            }
                            //Run Application hook
                            if (RunApplication != null)
                                await RunApplication();
                            else
                                await Task.Delay(500);
                        }
                        if (_interruptApplication)
                            _logging.LogMessage("Leaving Logic Loop - Manual interruption", LoggingLevel.Verbose);
                        else if (!_azureIoTHubService.Connected)
                            _logging.LogMessage("Leaving Logic Loop - Disconnected", LoggingLevel.Verbose);
                        else
                            _logging.LogMessage("Leaving Logic Loop - Unknown reason", LoggingLevel.Critical);
                    }
                    else
                    {
                        _logging.LogMessage("The connection to IoT Hub did not succeed. Please make sure that the TPM service is properly set up on Device 0 and that the connection string is properly set up.", LoggingLevel.Error);
                        await DisconnectedApplicationInternal();
                    } 
                }
                _logging.LogMessage("Leaving Application Loop", LoggingLevel.Verbose);
            }
            catch (Exception e)
            {
                _logging.LogMessage($"General Running Error: '{e.Message}'", LoggingLevel.Critical);
                _logging.LogMessage($"Stacktrace: '{e.StackTrace}'", LoggingLevel.Critical);
                await GeneralErrorInternal();
            }

            //End application phase
            _logging.LogMessage("Exiting Application", LoggingLevel.Information);
            try
            {
                //End Application hook
                await EndApplicationInternal();

                //Program interrupted, dispose the services
                if (_azureIoTHubService != null)
                    _azureIoTHubService.Dispose();
                if (_gpioService != null)
                    _gpioService.Dispose();
            }
            catch (Exception e)
            {
                _logging.LogMessage($"General EndApplication Error: '{e.Message}'", LoggingLevel.Critical);
                _logging.LogMessage($"Stacktrace: '{e.StackTrace}'", LoggingLevel.Critical);
            }

            //Restart
            _logging.LogMessage("Restarting the application in 5 seconds...", LoggingLevel.Information);
            await Task.Delay(5000);
            await RunApplicationInternal();
        }

        private bool InitApplicationInternal()
        {
            if (_configurationLoaded)
                return true;

            try
            {
                _configurationLoaded = InitApplication != null ? InitApplication(_logging, _azureIoTHubService, _gpioService) : true;
                _azureIoTHubService.SetDirectMethodCallback("test", DirectMethodTest);
                return true;
            }
            catch(Exception e)
            {
                _logging.LogMessage($"Unhandled Error: '{e.Message}'", LoggingLevel.Critical);
                _logging.LogMessage($"Stacktrace: '{e.StackTrace}'", LoggingLevel.Critical);
                _configurationLoaded = false;
            }
            return false;
        }

        private async Task StartApplicationInternal()
        {
            if (StartApplication != null)
            {
                _logging.LogMessage("Retrieve desired properties", LoggingLevel.Verbose);
                var desiredProperties = await _azureIoTHubService.GetDeviceTwinAsync();
                if (string.IsNullOrEmpty(desiredProperties))
                {
                    _logging.LogMessage("Cannot retrieve desired properties", LoggingLevel.Error);
                    return;
                }

                D desired = JsonConvert.DeserializeObject<D>(desiredProperties);
                if (desired != null)
                {
                    R reported = await StartApplication(desired);
                    if (reported != null)
                        await _azureIoTHubService.UpdateReportedProperties(new TwinCollection(JsonConvert.SerializeObject(reported)));
                }
            }
        }

        private async Task EndApplicationInternal()
        {
            if (EndApplication != null)
                await EndApplication();
        }

        private async Task DisconnectedApplicationInternal()
        {
            if (DisconnectedApplication != null)
                await DisconnectedApplication();
            else
                await Task.Delay(500);
        }

        private async Task GeneralErrorInternal()
        {
            if (GeneralError != null)
            {
                try
                {
                    await GeneralError();
                }
                catch (Exception eg)
                {
                    _logging.LogMessage($"Error in GeneralError hook: '{eg.Message}'", LoggingLevel.Error);
                }
            }
        }

        protected void RestartApplication()
        {
            _interruptApplication = true;
        }

        protected async Task ReceiveDesiredConfiguration(TwinCollection desiredProperties, object userContext)
        {
            _logging.LogMessage("ReceiveDesiredConfiguration", LoggingLevel.Verbose);

            if (ChangeConfiguration != null)
            {
                D desired = JsonConvert.DeserializeObject<D>(desiredProperties.ToJson());
                if (desired != null)
                {
                    R reported = await ChangeConfiguration(desired);
                    if (reported != null)
                        await _azureIoTHubService.UpdateReportedProperties(new TwinCollection(JsonConvert.SerializeObject(reported)));
                }
            }
            RestartApplication();
        }

        private async Task<MethodResponse> DirectMethodTest(MethodRequest methodRequest, object userContext)
        {
            try
            {
                if (TestMethod != null)
                    await TestMethod();
                else
                    await _azureIoTHubService.SendIoTMessage(_eventTypeTest);
            }
            catch(Exception e)
            {
                _logging.LogMessage($"DirectMethodTest Error: '{e.Message}'", LoggingLevel.Error);
                _logging.LogMessage($"Stacktrace: '{e.StackTrace}'", LoggingLevel.Error);
            }
            return new MethodResponse(200);
        }
    }
}
