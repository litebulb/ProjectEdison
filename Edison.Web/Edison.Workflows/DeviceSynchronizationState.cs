using Automatonymous;
using Edison.Core.Common.Models;
using MassTransit.DocumentDbIntegration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Workflows
{
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
