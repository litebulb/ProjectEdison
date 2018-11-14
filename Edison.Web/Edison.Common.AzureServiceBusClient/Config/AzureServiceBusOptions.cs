namespace Edison.Common.Config
{
    public class AzureServiceBusOptions
    {
        public string ConnectionString { get; set; }
        public int PrefetchCount { get; set; }
    }
}
