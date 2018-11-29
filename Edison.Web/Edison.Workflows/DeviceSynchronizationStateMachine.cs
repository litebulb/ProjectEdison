using System;
using Microsoft.Extensions.Options;
using MassTransit;
using Automatonymous;
using Edison.Core.Common.Models;
using Edison.Common.Config;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;

namespace Edison.Workflows
{
    /// <summary>
    /// Saga to handle the lifecycle of an iot device synchronization update
    /// </summary>
    internal class DeviceSynchronizationStateMachine : MassTransitStateMachine<DeviceSynchronizationState>
    {
        //Warning keep concurrency limit to 1
        private readonly ServiceBusRabbitMQOptions _configBus;

        public DeviceSynchronizationStateMachine(IOptions<ServiceBusRabbitMQOptions> configBus)
        {
            _configBus = configBus.Value;

            InstanceState(x => x.State);

            #region Initialization Events
            //Devices Updates
            Event(() => EventReceived, x => x.CorrelateById(context => context.Message.DeviceId).SelectId(context => Guid.NewGuid()));
            Event(() => EventDeviceDeleted, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => DeviceCreatedOrUpdated, x => x.CorrelateById(context => context.Message.CorrelationId));
            //UI Update notification, can be ignored if saga doesn't exist anymore
            Event(() => DeviceUIUpdated, x => { x.CorrelateById(context => context.Message.CorrelationId); x.OnMissingInstance(m => m.Discard()); });

            Event(() => DeviceDeleteRequestedFault, x => x.CorrelateById(context => context.Message.Message.CorrelationId));
            Event(() => DeviceCreateOrUpdateRequestedFault, x => x.CorrelateById(context => context.Message.Message.CorrelationId));
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
                        .ThenAsync(context => context.Publish(new DeviceCreateOrUpdateRequestedEvent()
                        {
                            CorrelationId = context.Instance.CorrelationId,
                            DeviceId = context.Data.DeviceId,
                            ChangeType = context.Data.ChangeType,
                            Date = context.Data.Date,
                            Data = context.Data.Data
                        }))
                        .TransitionTo(Waiting)
                    )
                    .If(new StateMachineCondition<DeviceSynchronizationState, IEventSagaReceivedDeviceChange>(bc =>
                        bc.Instance.ChangeType == "deleteDeviceIdentity"), x => x
                        .ThenAsync(context => context.Publish(new DeviceDeleteRequestedEvent()
                        {
                            CorrelationId = context.Instance.CorrelationId,
                            DeviceId = context.Data.DeviceId
                        }))
                        .TransitionTo(Waiting)
                    ));

            //State while the Device is being created / updated
            During(Waiting,
                When(DeviceCreatedOrUpdated)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"DeviceSynchronization---{context.Instance.CorrelationId}: Device Updated: {context.Instance.ChangeType}."))
                    .If(new StateMachineCondition<DeviceSynchronizationState, IDeviceCreatedOrUpdated>(bc => bc.Data.NotifyUI), x => x
                        .ThenAsync(context => context.Publish(new DeviceUIUpdateRequestedEvent()
                        {
                            CorrelationId = context.Instance.CorrelationId,
                            DeviceUI = new DeviceUIModel()
                            {
                                DeviceId = context.Instance.DeviceId,
                                Device = context.Data.Device,
                                UpdateType = context.Instance.ChangeType == "twinChangeNotification" || context.Instance.ChangeType == "ping"  ? DeviceUpdateType.UpdateDevice.ToString() : DeviceUpdateType.NewDevice.ToString()
                            }
                        }))
                    )
                    .Finalize(),
                When(EventDeviceDeleted)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"DeviceSynchronization---{context.Instance.CorrelationId}: Device Deleted: {context.Instance.DeviceId}."))
                    .ThenAsync(context => context.Publish(new DeviceUIUpdateRequestedEvent()
                    {
                        CorrelationId = context.Instance.CorrelationId,
                        DeviceUI = new DeviceUIModel()
                        {
                            DeviceId = context.Instance.DeviceId,
                            UpdateType = DeviceUpdateType.DeleteDevice.ToString()
                        }
                    }))
                    .Finalize()
                );

            //Stateless
            DuringAny(
                When(DeviceDeleteRequestedFault)
                .ThenAsync(context => Console.Out.WriteLineAsync($"DeviceSynchronization---{context.Instance.CorrelationId}: !!FAULT!! Device Deleted: {context.Data.Message}")),
                When(DeviceCreateOrUpdateRequestedFault)
                .ThenAsync(context => Console.Out.WriteLineAsync($"DeviceSynchronization---{context.Instance.CorrelationId}: !!FAULT!! Device Updated: {context.Data.Message}"))
            //    //UI Update acknoledgement
            //    When(DeviceUIUpdated)
            //        .ThenAsync(context => Console.Out.WriteLineAsync($"DeviceSynchronization---{context.Instance.CorrelationId}: UI Updated."))
                );


            //Delete persisted saga after completion
            SetCompletedWhenFinalized();
        }

        public State Waiting { get; private set; }

        #region "Events"
        public Event<IDeviceDeleted> EventDeviceDeleted { get; private set; }
        public Event<IDeviceCreatedOrUpdated> DeviceCreatedOrUpdated { get; private set; }
        public Event<Fault<IDeviceDeleteRequested>> DeviceDeleteRequestedFault { get; private set; }
        public Event<Fault<IDeviceCreateOrUpdateRequested>> DeviceCreateOrUpdateRequestedFault { get; private set; }
        public Event<IEventSagaReceivedDeviceChange> EventReceived { get; private set; }
        public Event<IDeviceUIUpdateRequested> DeviceUIUpdated { get; private set; }
        #endregion
    }
}
