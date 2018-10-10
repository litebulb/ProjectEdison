using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Workflows.Config
{
    public class WorkflowConfigDeviceSynchronization
    {
        public string DeviceSynchronizationQueue { get; set; }
        public int RequestTimeoutSeconds { get; set; }
    }
}
