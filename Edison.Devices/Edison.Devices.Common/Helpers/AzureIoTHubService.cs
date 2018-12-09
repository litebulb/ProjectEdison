using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Devices.Tpm;
using Newtonsoft.Json;
using Windows.Foundation.Diagnostics;

namespace Edison.Devices.Common
{
    public class AzureIoTHubService : IDisposable
    {
        private readonly LoggingChannel _logging;
        private DeviceClient _deviceClient;
        private DesiredPropertyUpdateCallback _callbackDesired;
        private List<DirectMethodConfig> _callbackDirectMethods;
        private const uint DEVICE_OPERATION_TEST_TIMEOUT = 10000;
        private const uint DEVICE_OPERATION_TIMEOUT = 20000;

        /// <summary>
        /// Indicate the connection state of the device
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// Main Constructor
        /// </summary>
        public AzureIoTHubService(Guid providerId)
        {
            _logging = new LoggingChannel("AzureIoTHubService", null, providerId);
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

        private X509Certificate2 GetDeviceCertificate(string deviceId)
        {
            X509Store storeMy = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            storeMy.Open(OpenFlags.ReadWrite);
            X509Certificate2 deviceCertificate = null;
            foreach (X509Certificate2 certificate in storeMy.Certificates)
            {
                if (certificate.Subject == $"CN={deviceId}")
                {
                    deviceCertificate = certificate;
                    break;
                }
            }
            storeMy.Close();
            storeMy.Dispose();
            return deviceCertificate;
        }

        /// <summary>
        /// Initialize the connection to IoTHub using the device connection string
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Init()
        {
            if (Connected)
                return true;

            TpmDevice tpmDevice = new TpmDevice(0);
            string deviceId = tpmDevice.GetDeviceId();
            string hostname = tpmDevice.GetHostName();

            _logging.LogMessage("Initializing IoT Hub Connection", LoggingLevel.Verbose);
            if (string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(hostname))
            {
                _logging.LogMessage("Not DeviceId or Hostname found.", LoggingLevel.Error);
                return false;
            }
            try
            {
                X509Certificate2 deviceCertificate = GetDeviceCertificate(deviceId);
                //Connection through certificate
                if (deviceCertificate != null)
                {
                    IAuthenticationMethod authentication = new DeviceAuthenticationWithX509Certificate(deviceId, deviceCertificate);
                    _deviceClient = DeviceClient.Create(hostname, authentication, TransportType.Mqtt);
                }
                //Connection through TPM (fallback)
                else
                {
                    _logging.LogMessage($"The certificate for device {deviceId} was not found. Trying to connect with TPM Connection string", LoggingLevel.Warning);
                    try
                    {
                        _deviceClient = DeviceClient.CreateFromConnectionString(tpmDevice.GetConnectionString(), TransportType.Mqtt);
                    }
                    catch
                    {
                        _logging.LogMessage($"The connection could be established through TPM. The device cannot start. Make sure that the device was properly provisioned.", LoggingLevel.Error);
                        return false;
                    }
                }
                _deviceClient.SetRetryPolicy(new NoRetry());
                _deviceClient.SetConnectionStatusChangesHandler(HandleConnectionStatusChange);
                _deviceClient.OperationTimeoutInMilliseconds = DEVICE_OPERATION_TEST_TIMEOUT;
                await _deviceClient.OpenAsync();

                _logging.LogMessage("Registering Device Twin update callback", LoggingLevel.Verbose);
                if(_callbackDesired != null)
                    await _deviceClient.SetDesiredPropertyUpdateCallbackAsync(_callbackDesired, null);
                foreach (var callbackDirectMethod in _callbackDirectMethods)
                    await _deviceClient.SetMethodHandlerAsync(callbackDirectMethod.MethodName, callbackDirectMethod.CallbackDirectMethod, null);
                Connected = true;
                return true;
            }
            catch (Exception he) when (
            he is IotHubCommunicationException ||
            he is UnauthorizedException ||
            he is ClosedChannelException ||
            he is TimeoutException)
            {
                _logging.LogMessage($"Init Handled Exception: {he.Message}", LoggingLevel.Error);
                Connected = false;
            }
            catch (Exception e) 
            {
                _logging.LogMessage($"Init: {e.Message}", LoggingLevel.Critical);
                Connected = false;
                throw e;
            }
            finally{
                _deviceClient.OperationTimeoutInMilliseconds = DEVICE_OPERATION_TIMEOUT; //DeviceClient.DefaultOperationTimeoutInMilliseconds;
            }

            return false;
        }

        /// <summary>
        /// Handle connection changes and drop
        /// </summary>
        /// <param name="status"></param>
        /// <param name="reason"></param>
        private void HandleConnectionStatusChange(ConnectionStatus status, ConnectionStatusChangeReason reason)
        {
            if (Connected && status == ConnectionStatus.Disconnected)
            {
                _logging.LogMessage($"HandleConnectionStatusChange Disconnected: reason: {reason}", LoggingLevel.Verbose);
                Connected = false;
            }
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
                if(_deviceClient != null)
                {
                    var messageIoT = new Message();
                    messageIoT.Properties.Add("opType", "ping");
                    await _deviceClient.SendEventAsync(messageIoT);
                }
                else
                {
                    _logging.LogMessage($"SendPingMessage: DeviceClient was empty.", LoggingLevel.Error);
                }
            }
            catch (Exception he) when (
            he is IotHubCommunicationException ||
            he is UnauthorizedException ||
            he is TimeoutException)
            {
                _logging.LogMessage($"SendPingMessage Handled Exception: {he.Message}", LoggingLevel.Error);
                Connected = false;
            }
            catch (Exception e)
            {
                _logging.LogMessage($"SendPingMessage: {e.Message}", LoggingLevel.Critical);
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
                if (_deviceClient != null)
                {
                    string messageJson = JsonConvert.SerializeObject(message);

                    var messageIoT = new Message(Encoding.UTF8.GetBytes(messageJson));
                    messageIoT.Properties.Add("opType", "eventDevice");
                    messageIoT.Properties.Add("eventType", eventType);

                    await _deviceClient.SendEventAsync(messageIoT);
                }
                else
                {
                    _logging.LogMessage($"SendIoTMessage: DeviceClient was empty.", LoggingLevel.Error);
                }
            }
            catch (Exception he) when (
            he is IotHubCommunicationException ||
            he is UnauthorizedException ||
            he is TimeoutException)
            {
                _logging.LogMessage($"SendIoTMessage Handled Exception: {he.Message}", LoggingLevel.Error);
                Connected = false;
            }
            catch (Exception e)
            {
                _logging.LogMessage($"SendIoTMessage: {e.Message}", LoggingLevel.Critical);
                Connected = false;
                throw e;
            }
        }

        public async Task UpdateReportedProperties(TwinCollection twin)
        {
            _logging.LogMessage("Update Reported Properties", LoggingLevel.Verbose);

            try
            {
                if (_deviceClient != null)
                {
                    await _deviceClient.UpdateReportedPropertiesAsync(twin);
                }
                else
                {
                    _logging.LogMessage($"UpdateReportedProperties: DeviceClient was empty.", LoggingLevel.Error);
                }
            }
            catch (Exception he) when (
            he is IotHubCommunicationException ||
            he is UnauthorizedException ||
            he is TimeoutException)
            {
                _logging.LogMessage($"UpdateReportedProperties Handled Exception: {he.Message}", LoggingLevel.Error);
                Connected = false;
            }
            catch (Exception e)
            {
                _logging.LogMessage($"UpdateReportedProperties: {e.Message}", LoggingLevel.Critical);
                Connected = false;
                throw e;
            }
        }

        public async Task SendIoTMessage(string eventType)
        {
            _logging.LogMessage("Send IoT message", LoggingLevel.Verbose);

            try
            {
                if (_deviceClient != null)
                {
                    var messageIoT = new Message();
                    messageIoT.Properties.Add("opType", "eventDevice");
                    messageIoT.Properties.Add("eventType", eventType);

                    await _deviceClient.SendEventAsync(messageIoT);
                }
                else
                {
                    _logging.LogMessage($"SendIoTMessage: DeviceClient was empty.", LoggingLevel.Error);
                }
            }
            catch (Exception he) when (
            he is IotHubCommunicationException ||
            he is UnauthorizedException ||
            he is TimeoutException)
            {
                _logging.LogMessage($"SendIoTMessage Handled Exception: {he.Message}", LoggingLevel.Error);
                Connected = false;
            }
            catch (Exception e)
            {
                _logging.LogMessage($"SendIoTMessage: {e.Message}", LoggingLevel.Critical);
                Connected = false;
                throw e;
            }
        }

        public async Task<string> GetDeviceTwinAsync()
        {
            _logging.LogMessage("Getting device twin", LoggingLevel.Verbose);

            try
            {
                if (_deviceClient != null)
                {
                    Twin twin = await _deviceClient.GetTwinAsync();
                    _logging.LogMessage(twin.ToJson(), LoggingLevel.Verbose);
                    return twin.Properties.Desired.ToJson();
                }
                else
                {
                    _logging.LogMessage($"GetDeviceTwinAsync: DeviceClient was empty.", LoggingLevel.Error);
                    return null;
                }
            }
            catch (Exception he) when (
            he is IotHubCommunicationException ||
            he is UnauthorizedException ||
            he is TimeoutException)
            {
                _logging.LogMessage($"GetDeviceTwinAsync Handled Exception: {he.Message}", LoggingLevel.Error);
                Connected = false;
                return null;
            }
            catch (Exception e)
            {
                _logging.LogMessage($"GetDeviceTwinAsync: {e.Message}", LoggingLevel.Critical);
                Connected = false;
                throw e;
            }
        }

        public void Dispose()
        {
            if (_deviceClient != null)
            {
                _deviceClient.CloseAsync();
                _deviceClient.Dispose();
                _deviceClient = null;
                Connected = false;
            }
        }
    }
}