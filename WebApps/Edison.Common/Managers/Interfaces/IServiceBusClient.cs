using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Common.Managers
{
    public interface IServiceBusClient
    {
        void Start(Action<IReceiveEndpointConfigurator> configurator, string queueName);
        IBus BusAccess { get; }
    }
}
