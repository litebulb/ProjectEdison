using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Edison.Common.Interfaces;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;
using Edison.MessageDispatcherService.Config;

namespace Edison.MessageDispatcherService
{
    /// <summary>
    /// Service to handle messages coming from Azure Service Bus
    /// </summary>
    public class MessageDispatcherService
    {
        private readonly ILogger<MessageDispatcherService> _logger;
        private readonly IAzureServiceBusClient _azureBusManagerClient;
        private readonly IMassTransitServiceBus _serviceBus;
        private readonly MessageDispatcherOptions _config;

        public MessageDispatcherService(IOptions<MessageDispatcherOptions> config,
            IAzureServiceBusClient azureBusManagerClient,
            IMassTransitServiceBus serviceBus, ILogger<MessageDispatcherService> logger)
        {
            _logger = logger;
            _azureBusManagerClient = azureBusManagerClient;
            _serviceBus = serviceBus;
            _config = config.Value;
        }

        public void Start()
        {
            _azureBusManagerClient.Start(HandleEventTriggerInput);
        }

        public async Task HandleEventTriggerInput(IDictionary<string, object> properties, DateTime date, string body)
        {
            _logger.LogDebug($"MessageDispatcherService.HandleEventTriggerInput: Retrieved message: {body}");

            //Message empty, return
            if (properties == null)
                return;

            //DeviceId is mandatory
            string deviceId = GetPropertyDeviceId(properties);
            if (!string.IsNullOrEmpty(deviceId))
            {
                string messageType = GetPropertyMessageType(properties);
                string eventType = GetPropertyEventType(properties);

                switch (messageType)
                {
                    case "eventDevice":
                        await PushMessageToEventProcessorSaga(deviceId, eventType, date, body);
                        break;
                    case "ping":
                        await PushMessageToDeviceManagementService(deviceId, messageType, date, body);
                        break;
                    case "createDeviceIdentity":
                        await PushMessageToDeviceManagementService(deviceId, messageType, date, body);
                        break;
                    case "deleteDeviceIdentity":
                        await PushMessageToDeviceManagementService(deviceId, messageType, date, body);
                        break;
                    case "updateTwin":
                        _logger.LogInformation($"Routing Twin Message.");
                        await PushMessageToDeviceManagementService(deviceId, eventType, date, body);
                        break;
                    default:
                        _logger.LogInformation($"Message Type '{messageType}' does not have a route. Skipping message.");
                        break;
                }
            }
            else
                _logger.LogInformation("DeviceId not found. Skipping message.");
        }

        private async Task<bool> PushMessageToEventProcessorSaga(string deviceId, string eventType, DateTime date, string body)
        {
            try
            {
                _logger.LogDebug($"MessageDispatcherService.PushMessageToEventProcessorSaga: Pushing message for device '{deviceId}', device type '{eventType}'.");
                IEventSagaReceived newMessage = new EventSagaReceivedEvent()
                {
                    DeviceId = new Guid(deviceId),
                    EventType = eventType,
                    Date = date,
                    Data = body
                };
                await _serviceBus.BusAccess.Publish(newMessage);
            }
            catch (Exception e)
            {
                _logger.LogError($"MessageDispatcherService.PushMessageToEventProcessorSaga: {e.Message}");
                return false;
            }
            return true;
        }

        private async Task<bool> PushMessageToDeviceManagementService(string deviceId, string changeType, DateTime date, string body)
        {
            try
            {
                _logger.LogDebug($"MessageDispatcherService.PushMessageToDeviceManagementSaga: Pushing message for device '{deviceId}', change type '{changeType}'.");
                IEventSagaReceivedDeviceChange newMessage = new DeviceSagaReceivedChangeEvent()
                {
                    DeviceId = new Guid(deviceId),
                    ChangeType = changeType,
                    Date = date,
                    Data = body
                };
                await _serviceBus.BusAccess.Publish(newMessage);
            }
            catch (Exception e)
            {
                _logger.LogError($"MessageDispatcherService.PushMessageToDeviceManagementSaga: {e.Message}");
                return false;
            }
            return true;
        }

        private string GetPropertyDeviceId(IDictionary<string, object> properties)
        {
            foreach (string tryProperty in _config.PropertyDeviceId)
                if (properties.ContainsKey(tryProperty))
                    return properties[tryProperty].ToString();
            return string.Empty;
        }

        private string GetPropertyEventType(IDictionary<string, object> properties)
        {
            foreach (string tryProperty in _config.PropertyEventType)
                if (properties.ContainsKey(tryProperty))
                    return properties[tryProperty].ToString();
            return _config.DefaultEventType;
        }


        private string GetPropertyMessageType(IDictionary<string, object> properties)
        {
            foreach (string tryProperty in _config.PropertyMessageType)
                if (properties.ContainsKey(tryProperty))
                    return properties[tryProperty].ToString();
            return _config.DefaultMessageType;
        }
    }
}
