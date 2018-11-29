using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MassTransit;
using Edison.Core.Interfaces;
using Edison.Core.Common.Models;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;

namespace Edison.EventProcessorService.Consumers
{
    /// <summary>
    /// Masstransit consumer that handles the closure of an event cluster.
    /// </summary>
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
