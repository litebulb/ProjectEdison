using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edison.Common.Interfaces
{
    public interface IAzureServiceBusClient
    {
        void Start(Func<IDictionary<string, object>, DateTime, string, Task> messageHandler);
    }
}
