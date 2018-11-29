using System;
using Automatonymous;
using MassTransit.DocumentDbIntegration;
using Newtonsoft.Json;

namespace Edison.Workflows
{
    /// <summary>
    /// State object for the device synchronization saga
    /// </summary>
    internal class DeviceSynchronizationState : SagaStateMachineInstance, IVersionedSaga
    {
        public string State { get; set; }
        [JsonProperty("id")]
        public Guid CorrelationId { get; set; }
        public Guid DeviceId { get; set; }
        public string ChangeType { get; set; }
        public Guid? RequestId { get; set; }
        [JsonIgnore]
        public string ETag { get; set; }
    }
}
