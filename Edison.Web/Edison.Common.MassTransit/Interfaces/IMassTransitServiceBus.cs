using MassTransit;
using System;

namespace Edison.Common.Interfaces
{
    public interface IMassTransitServiceBus
    {
        void Start(Action<IReceiveEndpointConfigurator> configurator, bool useSchedulerEndpoint = false);
        IBus BusAccess { get; }
    }
}
