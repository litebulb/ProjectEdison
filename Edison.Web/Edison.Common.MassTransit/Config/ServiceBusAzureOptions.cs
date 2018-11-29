namespace Edison.Common.Config
{
    public class ServiceBusAzureOptions
    {
        public string ConnectionString { get; set; }
        public int OperationTimeoutSeconds { get; set; }
        public string QueueName { get; set; }
        public ushort PrefetchCount { get; set; }
        public ushort ConcurrencyLimit { get; set; }
    }
}
