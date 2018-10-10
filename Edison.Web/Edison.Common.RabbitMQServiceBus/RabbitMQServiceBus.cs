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
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Edison.Common
{
    public class RabbitMQServiceBus : IDisposable, IServiceBusClient
    {
        private IBusControl _bus;
        private readonly ServiceBusOptions _config;
        private readonly ILogger<RabbitMQServiceBus> _logger;
        private IScheduler _scheduler;
        private TelemetryClient _telemetryClient;

        public IBus BusAccess { get { return _bus; } }

        public RabbitMQServiceBus(IOptions<ServiceBusOptions> config,
            ILogger<RabbitMQServiceBus> logger)
        {
            _logger = logger;
            _config = config.Value;

            //// Adding JSON file into IConfiguration.
            //IConfiguration config = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json", true, true)
            //    .Build();

            // Read instrumentation key from IConfiguration.
            //string ikey = config["ApplicationInsights:InstrumentationKey"];

            TelemetryConfiguration.Active.InstrumentationKey = "1a07a889-5f1a-4598-ad48-9fb8b3339b97";
            _telemetryClient = new TelemetryClient();
            _telemetryClient.TrackTrace("Instantiating an instance of RabbitMQServiceBus class");
        }

        public void Start(Action<IReceiveEndpointConfigurator> configurator, bool useSchedulerEndpoint = false)
        {
            try
            {
                _logger.LogInformation("Starting BusManager...");
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
                _logger.LogError($"BusManager: {e.Message}");
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
