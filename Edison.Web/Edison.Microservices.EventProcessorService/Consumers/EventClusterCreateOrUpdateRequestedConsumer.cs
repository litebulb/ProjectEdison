using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using Edison.Core.Interfaces;
using Edison.EventProcessorService.Models;
using MassTransit;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Edison.EventProcessorService.Consumers
{
    public class EventClusterCreateOrUpdateRequestedConsumer : IConsumer<IEventClusterCreateOrUpdateRequested>
    {
        private readonly IEventClusterRestService _eventClusterRestService;
        private readonly ILogger<EventClusterCreateOrUpdateRequestedConsumer> _logger;

        public EventClusterCreateOrUpdateRequestedConsumer(IEventClusterRestService eventClusterRestService,
            ILogger<EventClusterCreateOrUpdateRequestedConsumer> logger)
        {
            _logger = logger;
            _eventClusterRestService = eventClusterRestService;
        }

        public async Task Consume(ConsumeContext<IEventClusterCreateOrUpdateRequested> context)
        {
            try
            {
                _logger.LogDebug($"EventClusterCreateRequestedConsumer: Retrieved message from device '{context.Message.DeviceId}'.");

                if (JsonConvert.DeserializeObject<DeviceTriggerIoTMessage>(context.Message.Data) is DeviceTriggerIoTMessage deviceMessage)
                {
                    EventClusterCreationModel newEvent = new EventClusterCreationModel()
                    {
                        EventClusterId = context.Message.EventClusterId,
                        Date = context.Message.Date,
                        EventType = context.Message.EventType.ToLower(),
                        DeviceId = context.Message.DeviceId,
                        Metadata = deviceMessage.Metadata,
                    };

                    var eventCluster = await _eventClusterRestService.CreateOrUpdateEventCluster(newEvent);
                    if (eventCluster != null && eventCluster.EventClusterId != Guid.Empty)
                    {
                        //Push message to Service bus queue
                        _logger.LogDebug($"EventClusterCreateRequestedConsumer: Event cluster created: '{eventCluster.EventClusterId}'.");
                        await context.Publish(new EventClusterCreatedOrUpdatedEvent() { EventCluster = eventCluster, LastEventDate = context.Message.Date });
                        return;
                    }

                    _logger.LogError("EventClusterCreateRequestedConsumer: The event could not be created.");
                    throw new Exception("The event cluster could not be created.");
                }

                _logger.LogError($"EventAddToClusterRequestedConsumer: Error while deserializing message from device '{context.Message.DeviceId}'.");
                throw new Exception($"Error while deserializing message from device '{context.Message.DeviceId}'.");
            }
            catch(Exception e)
            {
                _logger.LogError($"EventAddToClusterRequestedConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
