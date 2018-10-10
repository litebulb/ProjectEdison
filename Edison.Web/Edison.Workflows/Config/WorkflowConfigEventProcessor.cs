using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Workflows.Config
{
    public class WorkflowConfigEventProcessor
    {
        public int EventClusterLifespan { get; set; }
        public int EventClusterCooldown { get; set; }
    }
}
