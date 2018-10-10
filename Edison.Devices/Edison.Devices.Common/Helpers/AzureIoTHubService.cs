using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;

namespace Edison.Devices.Common
{
    public class AzureIoTHubService : IDisposable
    {
        private readonly LoggingChannel _logging;
        private DeviceClient _deviceClient;
        private DesiredPropertyUpdateCallback _callbackDesired;
        private List<DirectMethodConfig> _callbackDirectMethods;

        /// <summary>
        /// Indicate the connection state of the device
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// Main Constructor
        /// </summary>
        public AzureIoTHubService()
        {
            _logging = new LoggingChannel("AzureIoTHubService", null, new Guid("28bd9055-5356-42db-a373-b5ac533c8dbd"));
            _callbackDirectMethods = new List<DirectMethodConfig>();
        }

        /// <summary>
        /// Set a callback on desired property change
        /// </summary>
        /// <param name="callbackDesired">DesiredPropertyUpdateCallback object</param>
        public void SetDesiredPropertyCallback(DesiredPropertyUpdateCallback callbackDesired)
        {
            _callbackDesired = callbackDesired;
        }

        /// <summary>
        /// Set a callback on direct method
        /// </summary>
        /// <param name="callbackDirectMethod">MethodCallback object</param>
        /// <returns>Task</returns>
        public void SetDirectMethodCallback(string methodName, MethodCallback callbackDirectMethod)
        {
            _callbackDirectMethods.Add(new DirectMethodConfig()
            {
                MethodName = methodName,
                CallbackDirectMethod = callbackDirectMethod
            });
        }

        /// <summary>
        /// Initialize the connection to IoTHub using the device connection string
        /// </summary>
        /// <param name="deviceConnectionString">Device connection string</param>
        /// <returns></returns>
        public async Task<bool> Init(string deviceConnectionString)
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
                if(_callbackDesired != null)
                    await _deviceClient.SetDesiredPropertyUpdateCallbackAsync(_callbackDesired, null);
                foreach (var callbackDirectMethod in _callbackDirectMethods)
                    await _deviceClient.SetMethodHandlerAsync(callbackDirectMethod.MethodName, callbackDirectMethod.CallbackDirectMethod, null);

                Connected = true;
            }
            catch(Exception e)
            {
                _logging.LogMessage($"Init: {e.Message}", LoggingLevel.Error);
                Connected = false;
                throw e;
            }

            return true;
        }

        /// <summary>
        /// Send a ping message with message property "opType" set to "ping"
        /// </summary>
        /// <returns></returns>
        public async Task SendPingMessage()
        {
            _logging.LogMessage("Send ping message", LoggingLevel.Verbose);

            try
            {
                var messageIoT = new Message();
                messageIoT.Properties.Add("opType", "ping");
                await _deviceClient.SendEventAsync(messageIoT);
            }
            catch (IotHubCommunicationException ce)
            {
                _logging.LogMessage($"SendPingMessage IotHubCommunicationException: {ce.Message}", LoggingLevel.Error);
                Connected = false;
                throw ce;
            }
            catch (Exception e)
            {
                _logging.LogMessage($"SendPingMessage: {e.Message}", LoggingLevel.Error);
                Connected = false;
                throw e;
            }

            _logging.LogMessage("Sent ping message", LoggingLevel.Verbose);
        }

        public async Task SendIoTMessage(object message, string eventType)
        {
            _logging.LogMessage("Send IoT message", LoggingLevel.Verbose);

            try
            {
                string messageJson = JsonConvert.SerializeObject(message);

                var messageIoT = new Message(Encoding.UTF8.GetBytes(messageJson));
                messageIoT.Properties.Add("opType", "eventDevice");
                messageIoT.Properties.Add("eventType", eventType);

                await _deviceClient.SendEventAsync(messageIoT);
            }
            catch (IotHubCommunicationException ce)
            {
                _logging.LogMessage($"SendIoTMessage IotHubCommunicationException: {ce.Message}", LoggingLevel.Error);
                Connected = false;
                throw ce;
            }
            catch (Exception e)
            {
                _logging.LogMessage($"SendIoTMessage: {e.Message}", LoggingLevel.Error);
                Connected = false;
                throw e;
            }
        }

        public async Task SendIoTMessage(string eventType)
        {
            _logging.LogMessage("Send IoT message", LoggingLevel.Verbose);

            try
            {
                var messageIoT = new Message();
                messageIoT.Properties.Add("opType", "eventDevice");
                messageIoT.Properties.Add("eventType", eventType);

                await _deviceClient.SendEventAsync(messageIoT);
            }
            catch (IotHubCommunicationException ce)
            {
                _logging.LogMessage($"SendIoTMessage IotHubCommunicationException: {ce.Message}", LoggingLevel.Error);
                Connected = false;
                throw ce;
            }
            catch (Exception e)
            {
                _logging.LogMessage($"SendIoTMessage: {e.Message}", LoggingLevel.Error);
                Connected = false;
                throw e;
            }
        }

        public async Task<string> GetDeviceTwinAsync()
        {
            _logging.LogMessage("Getting device twin", LoggingLevel.Verbose);

            try
            {
                Twin twin = await _deviceClient.GetTwinAsync();
                _logging.LogMessage(twin.ToJson(), LoggingLevel.Verbose);
                return twin.Properties.Desired.ToJson();
            }
            catch (IotHubCommunicationException ce)
            {
                _logging.LogMessage($"GetDeviceTwinAsync IotHubCommunicationException: {ce.Message}", LoggingLevel.Error);
                Connected = false;
                throw ce;
            }
            catch (Exception e)
            {
                _logging.LogMessage($"GetDeviceTwinAsync: {e.Message}", LoggingLevel.Error);
                Connected = false;
                throw e;
            }
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