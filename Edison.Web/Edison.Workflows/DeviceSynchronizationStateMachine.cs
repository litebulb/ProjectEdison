using Automatonymous;
using Edison.Common.Config;
using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using Edison.Workflows.Config;
using MassTransit;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Edison.Workflows
{
    internal class DeviceSynchronizationStateMachine : MassTransitStateMachine<DeviceSynchronizationState>
    {
        //Warning keep concurrency limit to 1
        private ServiceBusOptions _configBus;
        private WorkflowConfigDeviceSynchronization _configWorkflow;

        public DeviceSynchronizationStateMachine(IOptions<ServiceBusOptions> configBus, IOptions<WorkflowConfig> configWorkflow)
        {
            _configBus = configBus.Value;
            _configWorkflow = configWorkflow.Value.DeviceSynchronizationWorkflow;

            InstanceState(x => x.State);

            #region Initialization Events
            //Devices Updates
            Event(() => EventReceived, x => x.CorrelateById(context => context.Message.DeviceId).SelectId(context => Guid.NewGuid()));
            //UI Update notification, can be ignored if saga doesn't exist anymore
            Event(() => DeviceUIUpdated, x => { x.CorrelateById(context => context.Message.CorrelationId); x.OnMissingInstance(m => m.Discard()); });

            Request(() => DeviceCreateOrUpdateRequest, x => x.RequestId,
                p =>
                {
                    p.Timeout = TimeSpan.FromSeconds(_configWorkflow.RequestTimeoutSeconds);
                    p.SchedulingServiceAddress = new Uri($"{_configBus.Uri}/{_configBus.QueueName}_scheduler");
                    p.ServiceAddress = new Uri($"{_configBus.Uri}/{_configWorkflow.DeviceSynchronizationQueue}");
                }
                );
            Request(() => DeviceDeleteRequest, x => x.RequestId,
                p =>
                {
                    p.Timeout = TimeSpan.FromSeconds(_configWorkflow.RequestTimeoutSeconds);
                    p.SchedulingServiceAddress = new Uri($"{_configBus.Uri}/{_configBus.QueueName}_scheduler");
                    p.ServiceAddress = new Uri($"{_configBus.Uri}/{_configWorkflow.DeviceSynchronizationQueue}");
                }
                );
            #endregion

            //The event is submitted from Azure Service Queue
            Initially(
                When(EventReceived)
                    .Then(context => context.Instance.DeviceId = context.Data.DeviceId)
                    .Then(context => context.Instance.ChangeType = context.Data.ChangeType)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"DeviceSynchronization-{context.Instance.CorrelationId}: Saga Initiated."))
                    .ThenAsync(context => Console.Out.WriteLineAsync($"DeviceSynchronization--{context.Instance.CorrelationId}: Event Received."))
                    .If(new StateMachineCondition<DeviceSynchronizationState, IEventSagaReceivedDeviceChange>(bc => 
                        bc.Instance.ChangeType == "twinChangeNotification" || bc.Instance.ChangeType == "createDeviceIdentity"
                         || bc.Instance.ChangeType == "ping"), x => x
                        .Request(DeviceCreateOrUpdateRequest, context => new DeviceCreateOrUpdateRequestedEvent()
                        {
                            DeviceId = context.Data.DeviceId,
                            ChangeType = context.Data.ChangeType,
                            Date = context.Data.Date,
                            Data = context.Data.Data
                        })
                        .TransitionTo(DeviceCreateOrUpdateRequest.Pending)
                    )
                    .If(new StateMachineCondition<DeviceSynchronizationState, IEventSagaReceivedDeviceChange>(bc =>
                        bc.Instance.ChangeType == "deleteDeviceIdentity"), x => x
                        .Request(DeviceDeleteRequest, context => new DeviceDeleteRequestedEvent()
                        {
                            DeviceId = context.Data.DeviceId
                        })
                        .TransitionTo(DeviceDeleteRequest.Pending)
                    ));

            //State while the Device is being created / updated
            During(DeviceCreateOrUpdateRequest.Pending,
                When(DeviceCreateOrUpdateRequest.TimeoutExpired)
                .ThenAsync(context => Console.Out.WriteLineAsync($"DeviceSynchronization---{context.Instance.CorrelationId}: !!TIMEOUT!! Device Synchronization." )),
                When(DeviceCreateOrUpdateRequest.Faulted)
                .ThenAsync(context => Console.Out.WriteLineAsync($"DeviceSynchronization---{context.Instance.CorrelationId}: !!FAULT!! Device Synchronization: {context.Data.Message}")),
                When(DeviceCreateOrUpdateRequest.Completed)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"DeviceSynchronization---{context.Instance.CorrelationId}: Device Updated: {context.Instance.ChangeType}."))
                    .ThenAsync(context => context.Publish(new DeviceUIUpdateRequestedEvent()
                    {
                        CorrelationId = context.Instance.CorrelationId,
                        DeviceUI = new DeviceUIModel()
                        {
                            DeviceId = context.Instance.DeviceId,
                            Device = context.Data.Device,
                            UpdateType = context.Instance.ChangeType == "twinChangeNotification" || context.Instance.ChangeType == "ping"  ? "UpdateDevice" : "NewDevice"
                        }
                    }))
                    .Finalize()
                );

            //State while the Device is being deleted
            During(DeviceDeleteRequest.Pending,
                When(DeviceDeleteRequest.TimeoutExpired)
                .ThenAsync(context => Console.Out.WriteLineAsync($"DeviceSynchronization---{context.Instance.CorrelationId}: !!TIMEOUT!! Device Synchronization.")),
                When(DeviceDeleteRequest.Faulted)
                .ThenAsync(context => Console.Out.WriteLineAsync($"DeviceSynchronization---{context.Instance.CorrelationId}: !!FAULT!! Device Synchronization: {context.Data.Message}")),
                When(DeviceDeleteRequest.Completed)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"DeviceSynchronization---{context.Instance.CorrelationId}: Device Deleted: {context.Instance.DeviceId}."))
                    .ThenAsync(context => context.Publish(new DeviceUIUpdateRequestedEvent()
                    {
                        CorrelationId = context.Instance.CorrelationId,
                        DeviceUI = new DeviceUIModel()
                        {
                            DeviceId = context.Instance.DeviceId,
                            UpdateType = "DeleteDevice"
                        }
                    }))
                    .Finalize()
                );

            //Stateless
            //DuringAny(
            //    //UI Update acknoledgement
            //    When(DeviceUIUpdated)
            //        .ThenAsync(context => Console.Out.WriteLineAsync($"DeviceSynchronization---{context.Instance.CorrelationId}: UI Updated."))
            //    );


            //Delete persisted saga after completion
            SetCompletedWhenFinalized();
        }


        public Request<DeviceSynchronizationState, IDeviceCreateOrUpdateRequested, IDeviceCreatedOrUpdated> DeviceCreateOrUpdateRequest { get; private set; }
        public Request<DeviceSynchronizationState, IDeviceDeleteRequested, IDeviceDeleted> DeviceDeleteRequest { get; private set; }


        #region "Events"
        public Event<IEventSagaReceivedDeviceChange> EventReceived { get; private set; }
        public Event<IDeviceUIUpdateRequested> DeviceUIUpdated { get; private set; }
        #endregion
    }
}
