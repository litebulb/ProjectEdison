using Edison.Common.Config;
using Edison.Common.Interfaces;
using MassTransit;
using Microsoft.Extensions.Options;
using System;
using GreenPipes;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using MassTransit.QuartzIntegration;

namespace Edison.Common
{
    public class ServiceBusRabbitMQ : IDisposable, IMassTransitServiceBus
    {
        private IBusControl _bus;
        private readonly ServiceBusRabbitMQOptions _config;
        private readonly ILogger<ServiceBusRabbitMQ> _logger;
        private IScheduler _scheduler;

        public IBus BusAccess { get { return _bus; } }

        public ServiceBusRabbitMQ(IOptions<ServiceBusRabbitMQOptions> config,
            ILogger<ServiceBusRabbitMQ> logger)
        {
            _logger = logger;
            _config = config.Value;
        }

        public void Start(Action<IReceiveEndpointConfigurator> configurator, bool useSchedulerEndpoint = false)
        {
            try
            {
                _logger.LogInformation("Starting RabbitMQBusManager...");
                if(useSchedulerEndpoint)
                    _scheduler = CreateScheduler();
                _bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
                {
                    var host = sbc.Host(new Uri(_config.Uri), h =>
                    {
                        h.Username(_config.Username);
                        h.Password(_config.Password);
                    });

                    if (!string.IsNullOrEmpty(_config.QueueName))
                    {
                        sbc.ReceiveEndpoint(host, _config.QueueName, ep =>
                        {
                            ep.PrefetchCount = _config.PrefetchCount;
                            ep.UseConcurrencyLimit(_config.ConcurrencyLimit);
                            configurator?.Invoke(ep);
                        });
                    }

                    if (useSchedulerEndpoint)
                    {
                        sbc.ReceiveEndpoint(host, $"{_config.QueueName}_scheduler", ep =>
                        {
                            ep.PrefetchCount = 1;
                            ep.UseConcurrencyLimit(10);
                            sbc.UseMessageScheduler(ep.InputAddress);

                            ep.Consumer(() => new ScheduleMessageConsumer(_scheduler));
                            ep.Consumer(() => new CancelScheduledMessageConsumer(_scheduler));
                        });
                    }
                });

                if (useSchedulerEndpoint)
                {
                    _scheduler.JobFactory = new MassTransitJobFactory(_bus);
                    _scheduler.Start();
                }
                _bus.Start();
            }
            catch (Exception e)
            {
                _logger.LogError($"RabbitMQBusManager: {e.Message}");
            }
        }

        public void Dispose()
        {
            if (_bus != null)
            {
                _bus.Stop();
            }
            if(_scheduler != null)
            {
                _scheduler.Standby();
                _scheduler.Shutdown();
            }
        }

        public IScheduler CreateScheduler()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = MassTransit.Util.TaskUtil.Await<IScheduler>(() => schedulerFactory.GetScheduler()); ;

            return scheduler;
        }
    }
}
