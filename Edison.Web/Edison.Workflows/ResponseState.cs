using System;
using Automatonymous;
using MassTransit.DocumentDbIntegration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Workflows
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ResponseUpdateType
    {
        NewResponse,
        UpdateResponseActions,
        ResponseActionCallback,
        UpdateResponse,
        CloseResponse
    }

    /// <summary>
    /// State object for the response saga
    /// </summary>
    internal class ResponseState : SagaStateMachineInstance, IVersionedSaga
    {
        public string State { get; set; }
        [JsonProperty("id")]
        public Guid CorrelationId { get; set; }
        public Guid ActionCorrelationId { get; set; }
        public int ActionsTotal { get; set; }
        public int ActionsCompletedCount { get; set; }
        public ResponseUpdateType ActionUpdateType { get; set; }
        [JsonIgnore]
        public string ETag { get; set; }
    }
}
