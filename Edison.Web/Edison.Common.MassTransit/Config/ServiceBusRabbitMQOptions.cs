using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Common.Config
{
    public class ServiceBusRabbitMQOptions
    {
        public string Uri { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
        public ushort PrefetchCount { get; set; }
        public ushort ConcurrencyLimit { get; set; }
    }
}
