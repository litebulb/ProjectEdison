using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Workflows.Config
{
    public class WorkflowConfig
    {
        public WorkflowConfigEventProcessor EventProcessingWorkflow { get; set; }
        public WorkflowConfigDeviceSynchronization DeviceSynchronizationWorkflow { get; set; }
        //public WorkflowConfigResponse ResponseWorkflow { get; set; }
    }
}
