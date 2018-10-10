using Automatonymous;
using Edison.Core.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Workflows
{
    internal class ResponseState : SagaStateMachineInstance
    {
        public string State { get; set; }
        [JsonProperty("id")]
        public Guid CorrelationId { get; set; }
        public ResponseModel Response { get; set; }
        public bool ActionPlanStartTriggered { get; set; }
        public bool ActionPlanCloseTriggered { get; set; }
        [JsonIgnore]
        public string ETag { get; set; }
    }
}
