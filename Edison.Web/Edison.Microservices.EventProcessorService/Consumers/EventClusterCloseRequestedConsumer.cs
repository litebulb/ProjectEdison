using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using Edison.Core.Interfaces;
using MassTransit;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Edison.EventProcessorService.Consumers
{
    public class EventClusterCloseRequestedConsumer : IConsumer<IEventClusterCloseRequested>
    {
        private readonly IEventClusterRestService _eventClusterRestService;
        private readonly ILogger<EventClusterCloseRequestedConsumer> _logger;

        public EventClusterCloseRequestedConsumer(IEventClusterRestService eventClusterRestService, ILogger<EventClusterCloseRequestedConsumer> logger)
        {
            _logger = logger;
            _eventClusterRestService = eventClusterRestService;
        }

        public async Task Consume(ConsumeContext<IEventClusterCloseRequested> context)
        {
            try
            {
                _logger.LogDebug($"EventClusterCloseRequestedConsumer: Retrieved close message for event cluster '{context.Message.EventClusterId}'");

                EventClusterCloseModel closeEvent = new EventClusterCloseModel()
                {
                    EventClusterId = context.Message.EventClusterId,
                    ClosureDate = context.Message.ClosureDate,
                    EndDate = context.Message.EndDate
                };

                var eventCluster = await _eventClusterRestService.CloseEventCluster(closeEvent);
                if (eventCluster != null && eventCluster.EventClusterId != Guid.Empty)
                {
                    //Push message to Service bus queue
                    _logger.LogDebug($"EventClusterCloseRequestedConsumer: Event closed successfully.");
                    await context.Publish(new EventClusterClosedEvent() { EventCluster = eventCluster });
                    return;
                }
                _logger.LogError("EventClusterCloseRequestedConsumer: The event cluster could not be closed.");
                throw new Exception("The event cluster could not be closed.");
            }
            catch (Exception e)
            {
                _logger.LogError($"EventClusterCloseRequestedConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
