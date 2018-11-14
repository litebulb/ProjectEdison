using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using Edison.Core.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edison.EventProcessorService.Consumers
{
    public class EventClusterCreateOrUpdateRequestedConsumer : IConsumer<IEventClusterCreateOrUpdateRequested>
    {
        private readonly IEventClusterRestService _eventClusterRestService;
        private readonly IDeviceRestService _deviceRestService;
        private readonly ILogger<EventClusterCreateOrUpdateRequestedConsumer> _logger;

        public EventClusterCreateOrUpdateRequestedConsumer(IEventClusterRestService eventClusterRestService,
            IDeviceRestService deviceRestService,
            ILogger<EventClusterCreateOrUpdateRequestedConsumer> logger)
        {
            _logger = logger;
            _eventClusterRestService = eventClusterRestService;
            _deviceRestService = deviceRestService;
        }

        public async Task Consume(ConsumeContext<IEventClusterCreateOrUpdateRequested> context)
        {
            try
            {
                _logger.LogDebug($"EventClusterCreateRequestedConsumer: Retrieved message from source '{context.Message.DeviceId}'.");

                if (context.Message.CheckBoundary)
                {
                    _logger.LogDebug($"EventClusterCreateRequestedConsumer: '{context.Message.DeviceId}' event must be checked for boundaries.");
                    if (!await _deviceRestService.IsInBoundaries(new DeviceBoundaryGeolocationModel()
                    {
                        DeviceId = context.Message.DeviceId
                    }))
                    {
                        _logger.LogError("EventClusterCreateRequestedConsumer: The event was not triggered within the geolocation boundaries.");
                        return;
                    }
                }


                if (JsonConvert.DeserializeObject<Dictionary<string,object>>(context.Message.Data) is Dictionary<string,object> deviceMessage)
                {
                    EventClusterCreationModel newEvent = new EventClusterCreationModel()
                    {
                        EventClusterId = context.Message.EventClusterId,
                        Date = context.Message.Date,
                        EventType = context.Message.EventType.ToLower(),
                        DeviceId = context.Message.DeviceId,
                        Metadata = deviceMessage,
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
