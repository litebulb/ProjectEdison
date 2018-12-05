using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using Edison.Core.Interfaces;
using Edison.EventProcessorService.Consumers;
using Edison.Tests.Common.Helpers;
using MassTransit;
using MassTransit.TestFramework;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Tests.Consumers
{
    #pragma warning disable 1998
    public class EventClusterCreateRequestedConsumerFixture : InMemoryTestFixture
    {
        protected Mock<ILogger<EventClusterCreateOrUpdateRequestedConsumer>> _mockLogger;
        protected Task<ConsumeContext<IEventClusterCreatedOrUpdated>> _eventClusterCreated;
        protected Task<ConsumeContext<Fault<IEventClusterCreateOrUpdateRequested>>> _eventClusterCreatedAddedFault;
        protected Mock<IEventClusterRestService> _mockEventRest;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _mockEventRest = new Mock<IEventClusterRestService>(MockBehavior.Strict);
            configurator.Consumer(() => new EventClusterCreateOrUpdateRequestedConsumer(_mockEventRest.Object, _mockLogger.Object));
            _eventClusterCreated = Handled<IEventClusterCreatedOrUpdated>(configurator);
            _eventClusterCreatedAddedFault = Handled<Fault<IEventClusterCreateOrUpdateRequested>>(configurator);

            //Mock
            _mockEventRest.Setup(p => p.CreateOrUpdateEventCluster(It.IsAny<EventClusterCreationModel>())).Returns<EventClusterCreationModel>((p) => Task.FromResult(MockCreateOrUpdateEventCluster(p)));
        }

        [OneTimeSetUp]
        public virtual async Task SetUp()
        {
            _mockLogger = LoggerHelper.CreateLogger<EventClusterCreateOrUpdateRequestedConsumer>();
        }

        protected EventClusterModel MockCreateOrUpdateEventCluster(EventClusterCreationModel model)
        {
            if (model == null)
                return null;

            if (model.EventClusterId == Guid.Empty && model.DeviceId == Guid.Empty)
                return new EventClusterModel();

            return new EventClusterModel()
            {
                EventClusterId = model.EventClusterId != Guid.Empty ? model.EventClusterId : Guid.Empty
            };
        }

        protected string CreateDeviceTriggerIoTMessage(IDictionary<string, object> eventMetadata)
        {
            dynamic newObject = new ExpandoObject();
            if (eventMetadata != null)
                newObject.Metadata = eventMetadata;

            return JsonConvert.SerializeObject(newObject);
        }
    }

    [TestFixture]
    public class ShouldSucceedNewCluster : EventClusterCreateRequestedConsumerFixture
    {
        public override async Task SetUp()
        {
            await base.SetUp();
            await InputQueueSendEndpoint.Send(new EventClusterCreateOrUpdateRequestedEvent()
            {
                EventClusterId = Guid.NewGuid(),
                DeviceId = new Guid("c337f50b-134a-4d83-8f40-18f6691e4dbb"),
                Data = CreateDeviceTriggerIoTMessage(null)
            });
        }

        [Test]
        public async Task TestShouldSucceedNewCluster()
        {
            await Task.WhenAny(_eventClusterCreated, _eventClusterCreatedAddedFault);

            Assert.IsTrue(_eventClusterCreated.IsCompleted);
            Assert.IsFalse(_eventClusterCreatedAddedFault.IsCompleted);

            Assert.True(LoggerHelper.VerifyDebugLog("Retrieved message from source 'c337f50b-134a-4d83-8f40-18f6691e4dbb'", _mockLogger));
            Assert.True(LoggerHelper.VerifyDebugLog("Event cluster created", _mockLogger));
        }
    }

    [TestFixture]
    public class ShouldFailClusterEmpty : EventClusterCreateRequestedConsumerFixture
    {
        public override async Task SetUp()
        {
            await base.SetUp();
            await InputQueueSendEndpoint.Send(new EventClusterCreateOrUpdateRequestedEvent()
            {
                EventClusterId = Guid.Empty,
                DeviceId = Guid.Empty,
                Data = CreateDeviceTriggerIoTMessage(null)
            });
        }

        [Test]
        public async Task TestShouldFailClusterEmpty()
        {
            await Task.WhenAny(_eventClusterCreated, _eventClusterCreatedAddedFault);

            Assert.False(_eventClusterCreated.IsCompleted);
            Assert.IsTrue(_eventClusterCreatedAddedFault.IsCompleted);

            Assert.True(LoggerHelper.VerifyErrorLog("The event could not be created", _mockLogger));
        }
    }

    [TestFixture]
    public class ShouldFailClusterNull : EventClusterCreateRequestedConsumerFixture
    {
        public override async Task SetUp()
        {
            await base.SetUp();
            await InputQueueSendEndpoint.Send(new EventClusterCreateOrUpdateRequestedEvent());
        }

        [Test]
        public async Task TestShouldFailClusterNull()
        {
            await Task.WhenAny(_eventClusterCreated, _eventClusterCreatedAddedFault);

            Assert.False(_eventClusterCreated.IsCompleted);
            Assert.IsTrue(_eventClusterCreatedAddedFault.IsCompleted);

            Assert.True(LoggerHelper.VerifyErrorLog("Value cannot be null.\r\nParameter name: value", _mockLogger));
        }
    }
}
