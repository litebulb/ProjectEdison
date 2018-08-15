using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;

namespace Edison.SoundSensor
{
    internal class AzureIoTHubService : IDisposable
    {
        private readonly LoggingChannel _logging;
        private DeviceClient _deviceClient;
        private DesiredPropertyUpdateCallback _callbackDesired;

        internal bool Connected { get; private set; }

        internal AzureIoTHubService(LoggingChannel logging, DesiredPropertyUpdateCallback callbackDesired)
        {
            _logging = logging;
            _callbackDesired = callbackDesired;
        }

        internal async Task<bool> Init(string deviceConnectionString)
        {
            if (Connected)
                return true;

            _logging.LogMessage("Initializing IoT Hub Connection", LoggingLevel.Verbose);
            if (string.IsNullOrEmpty(deviceConnectionString))
            {
                _logging.LogMessage("Not DeviceConnectionString found.", LoggingLevel.Error);
                return false;
            }
            try
            {
                _deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);

                _logging.LogMessage("Registering Device Twin update callback", LoggingLevel.Verbose);
                await _deviceClient.SetDesiredPropertyUpdateCallbackAsync(_callbackDesired, null);

                Connected = true;
            }
            catch(Exception e)
            {
                _logging.LogMessage($"AzureIoTHubService - Init: {e.Message}", LoggingLevel.Critical);
                Connected = false;
            }

            return true;
        }

        internal async Task<string> GetDeviceTwinAsync()
        {
            _logging.LogMessage("Getting device twin", LoggingLevel.Verbose);
            Twin twin = await _deviceClient.GetTwinAsync();
            _logging.LogMessage(twin.ToJson(), LoggingLevel.Verbose);

            return twin.Properties.Desired.ToJson();
        }

        public void Dispose()
        {
            if (_deviceClient != null)
            {
                _deviceClient.Dispose();
                _deviceClient = null;
                Connected = false;
            }
        }
    }
}