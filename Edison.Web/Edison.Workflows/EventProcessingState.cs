using System;
using Automatonymous;
using MassTransit.DocumentDbIntegration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Workflows
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventClusterUpdateType
    {
        NewEventCluster,
        UpdateEventCluster,
        CloseEventCluster
    }

    /// <summary>
    /// State object for the event processing saga
    /// </summary>
    internal class EventProcessingState : SagaStateMachineInstance, IVersionedSaga
    {
        public string State { get; set; }
        public Guid DeviceId { get; set; }
        public DateTime LastEventReceived { get; set; }
        public string EventType { get; set; }
        public string SagaDeviceId { get; set; }
        [JsonProperty("id")]
        public Guid CorrelationId { get; set; }
        public Guid? ExpirationId { get; set; }
        [JsonIgnore]
        public string ETag { get; set; }
    }
}
