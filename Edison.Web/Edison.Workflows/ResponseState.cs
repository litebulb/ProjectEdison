using System;
using Automatonymous;
using MassTransit.DocumentDbIntegration;
using Newtonsoft.Json;
using Edison.Core.Common.Models;

namespace Edison.Workflows
{
    /// <summary>
    /// State object for the response saga
    /// </summary>
    internal class ResponseState : SagaStateMachineInstance, IVersionedSaga
    {
        public string State { get; set; }
        [JsonProperty("id")]
        public Guid CorrelationId { get; set; }
        public ResponseModel Response { get; set; }
        public bool ActionPlanCloseTriggered { get; set; }
        public int CloseActionsCount { get; set; }
        public int CloseActionsTotal { get; set; }
        [JsonIgnore]
        public string ETag { get; set; }
    }
}
