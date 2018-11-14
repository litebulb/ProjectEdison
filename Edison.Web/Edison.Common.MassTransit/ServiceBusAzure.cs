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
using MassTransit.Azure.ServiceBus.Core;

namespace Edison.Common
{
    public class ServiceBusAzure : IDisposable, IMassTransitServiceBus
    {
        private IBusControl _bus;
        private readonly ServiceBusAzureOptions _config;
        private readonly ILogger<ServiceBusAzure> _logger;
        private IScheduler _scheduler;

        public IBus BusAccess { get { return _bus; } }

        public ServiceBusAzure(IOptions<ServiceBusAzureOptions> config,
            ILogger<ServiceBusAzure> logger)
        {
            _logger = logger;
            _config = config.Value;
        }

        public void Start(Action<IReceiveEndpointConfigurator> configurator, bool useSchedulerEndpoint = false)
        {
            try
            {
                _logger.LogInformation("Starting AzureBusManager...");
                if(useSchedulerEndpoint)
                    _scheduler = CreateScheduler();
                _bus = Bus.Factory.CreateUsingAzureServiceBus(sbc =>
                {
                    var host = sbc.Host(_config.ConnectionString, h =>
                    {
                        h.OperationTimeout = TimeSpan.FromSeconds(_config.OperationTimeoutSeconds > 5 ? _config.OperationTimeoutSeconds : 5);
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
                _logger.LogError($"AzureBusManager: {e.Message}");
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
