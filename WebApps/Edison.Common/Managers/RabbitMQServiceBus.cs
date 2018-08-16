using Edison.Common;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Edison.Common.Config;
using Edison.Common.Managers;

namespace Edison.Common.Managers
{
    public class RabbitMQServiceBus : IDisposable, IServiceBusClient
    {
        private IBusControl _bus;
        private readonly ServiceBusOptions _config;
        private readonly ILogger<RabbitMQServiceBus> _logger;

        public IBus BusAccess { get { return _bus; } }

        public RabbitMQServiceBus(IOptions<ServiceBusOptions> config,
            ILogger<RabbitMQServiceBus> logger)
        {
            _logger = logger;
            _config = config.Value;
        }

        public void Start(Action<IReceiveEndpointConfigurator> configurator, string queueName)
        {
            try
            {
                _logger.LogInformation("Starting BusManager...");
                _bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
                {
                    var host = sbc.Host(new Uri(_config.Uri), h =>
                    {
                        h.Username(_config.Username);
                        h.Password(_config.Password);
                    });

                    if (!string.IsNullOrEmpty(queueName))
                    {
                        sbc.ReceiveEndpoint(host, queueName, ep =>
                        {
                            //ep.UseRetry(i => i.Exponential(3, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3)));
                            ep.PrefetchCount = 8;
                            configurator(ep);
                        });
                    }

                });

                _bus.Start();
            }
            catch (Exception e)
            {
                _logger.LogError($"BusManager: {e.Message}");
            }
        }

        public void Dispose()
        {
            if (_bus != null)
            {
                _bus.Stop();
            }
        }
    }
}
