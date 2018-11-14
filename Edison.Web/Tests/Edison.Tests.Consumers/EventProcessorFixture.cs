using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.EventProcessorService.Consumers;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Tests.Consumers
{
    #pragma warning disable 1998
    public class EventClusterCreateRequestedConsumerFixture : ConsumerFixtureBase
    {
        protected Mock<ILogger<EventClusterCreateOrUpdateRequestedConsumer>> _mockLogger;
        protected Task<ConsumeContext<IEventClusterCreatedOrUpdated>> _eventClusterCreated;
        protected Task<ConsumeContext<Fault<IEventClusterCreateOrUpdateRequested>>> _eventClusterCreatedAddedFault;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer(() => new EventClusterCreateOrUpdateRequestedConsumer(_mockEventRest.Object, _mockDeviceRest.Object, _mockLogger.Object));
            _eventClusterCreated = Handled<IEventClusterCreatedOrUpdated>(configurator);
            _eventClusterCreatedAddedFault = Handled<Fault<IEventClusterCreateOrUpdateRequested>>(configurator);
        }

        [OneTimeSetUp]
        public virtual async Task SetUp()
        {
            DBMock.Init();
            SetupMocks();
            _mockLogger = CreateLogger<EventClusterCreateOrUpdateRequestedConsumer>();
        }
    }

    public class EventClusterCloseRequestedConsumerFixture : ConsumerFixtureBase
    {
        protected Mock<ILogger<EventClusterCloseRequestedConsumer>> _mockLogger;
        protected Task<ConsumeContext<IEventClusterClosed>> _eventClusterClosed;
        protected Task<ConsumeContext<Fault<IEventClusterCloseRequested>>> _eventClusterClosedAddedFault;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer(() => new EventClusterCloseRequestedConsumer(_mockEventRest.Object, _mockLogger.Object));
            _eventClusterClosed = Handled<IEventClusterClosed>(configurator);
            _eventClusterClosedAddedFault = Handled<Fault<IEventClusterCloseRequested>>(configurator);
        }

        [OneTimeSetUp]
        public virtual async Task SetUp()
        {
            DBMock.Init();
            SetupMocks();
            _mockLogger = CreateLogger<EventClusterCloseRequestedConsumer>();
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
                EventType = "button",
                DeviceId = new Guid("c337f50b-134a-4d83-8f40-18f6691e4dbb"),
                Date = DateTime.UtcNow,
                Data = CreateDeviceTriggerIoTMessage(null)
            });
        }

        [Test]
        public async Task TestShouldSucceedNewCluster()
        {
            await Task.WhenAny(_eventClusterCreated, _eventClusterCreatedAddedFault);

            Assert.IsTrue(_eventClusterCreated.IsCompleted);
            Assert.IsFalse(_eventClusterCreatedAddedFault.IsCompleted);

            Assert.True(VerifyDebugLog("Retrieved message from device", _mockLogger));
            Assert.True(VerifyDebugLog("Event cluster created", _mockLogger));
            var events = await _mockEventRest.Object.GetEventClusters();
            var eventsButton = events.ToList().FindAll(p => p.EventType == "button");
            var eventsSound = events.ToList().FindAll(p => p.EventType == "sound");
            var eventsTest = events.ToList().FindAll(p => p.EventType == "test");
            Assert.AreEqual(eventsButton.Count, 3);
            Assert.AreEqual(eventsSound.Count, 1);
            Assert.AreEqual(eventsTest.Count, 0);
        }
    }

    [TestFixture]
    public class ShouldSucceedNewClusterEventTypeTest : EventClusterCreateRequestedConsumerFixture
    {
        public override async Task SetUp()
        {
            await base.SetUp();
            await InputQueueSendEndpoint.Send(new EventClusterCreateOrUpdateRequestedEvent()
            {
                EventClusterId = Guid.NewGuid(),
                EventType = "test",
                DeviceId = new Guid("fbc64b5c-ff21-4ade-9440-85f7b16ef01e"),
                Date = DateTime.UtcNow,
                Data = CreateDeviceTriggerIoTMessage(null)
            });
        }

        [Test]
        public async Task TestShouldSucceedNewClusterEventTypeTest()
        {
            await Task.WhenAny(_eventClusterCreated, _eventClusterCreatedAddedFault);

            Assert.IsTrue(_eventClusterCreated.IsCompleted);
            Assert.IsFalse(_eventClusterCreatedAddedFault.IsCompleted);

            Assert.True(VerifyDebugLog("Retrieved message from device", _mockLogger));
            Assert.True(VerifyDebugLog("Event cluster created", _mockLogger));
            var events = await _mockEventRest.Object.GetEventClusters();
            var eventsButton = events.ToList().FindAll(p => p.EventType == "button");
            var eventsSound = events.ToList().FindAll(p => p.EventType == "sound");
            var eventsTest = events.ToList().FindAll(p => p.EventType == "test");
            Assert.AreEqual(eventsButton.Count, 2);
            Assert.AreEqual(eventsSound.Count, 1);
            Assert.AreEqual(eventsTest.Count, 1);
        }
    }

    [TestFixture]
    public class ShouldSucceedNewClusterOverExisting : EventClusterCreateRequestedConsumerFixture
    {
        public override async Task SetUp()
        {
            await base.SetUp();
            await InputQueueSendEndpoint.Send(new EventClusterCreateOrUpdateRequestedEvent()
            {
                EventClusterId = Guid.NewGuid(),
                EventType = "button",
                DeviceId = new Guid("7776a948-90f8-4ffd-9578-f8078b07d96f"),
                Date = DateTime.UtcNow,
                Data = CreateDeviceTriggerIoTMessage(null)
            });
        }

        [Test]
        public async Task TestShouldSucceedNewClusterOverExisting()
        {
            await Task.WhenAny(_eventClusterCreated, _eventClusterCreatedAddedFault);

            Assert.IsTrue(_eventClusterCreated.IsCompleted);
            Assert.IsFalse(_eventClusterCreatedAddedFault.IsCompleted);

            Assert.True(VerifyDebugLog("Retrieved message from device", _mockLogger));
            Assert.True(VerifyDebugLog("Event cluster created", _mockLogger));
            var events = await _mockEventRest.Object.GetEventClusters();
            var eventsButton = events.ToList().FindAll(p => p.EventType == "button");
            var eventsSound = events.ToList().FindAll(p => p.EventType == "sound");
            var eventsTest = events.ToList().FindAll(p => p.EventType == "test");
            Assert.AreEqual(2, eventsButton.Count);
            Assert.AreEqual(eventsSound.Count, 1);
            Assert.AreEqual(eventsTest.Count, 0);
        }
    }

    [TestFixture]
    public class ShouldFailDeviceDoesntExist : EventClusterCreateRequestedConsumerFixture
    {
        public override async Task SetUp()
        {
            await base.SetUp();
            await InputQueueSendEndpoint.Send(new EventClusterCreateOrUpdateRequestedEvent()
            {
                EventClusterId = Guid.NewGuid(),
                EventType = "button",
                DeviceId = new Guid("f2a684cc-d2c2-4a25-9257-f8d5e09d2a3f"),
                Date = DateTime.UtcNow,
                Data = CreateDeviceTriggerIoTMessage(null)
            });
        }

        [Test]
        public async Task TestShouldFailDeviceDoesntExist()
        {
            await Task.WhenAny(_eventClusterCreated, _eventClusterCreatedAddedFault);

            Assert.IsTrue(_eventClusterCreatedAddedFault.IsCompleted);
            Assert.IsFalse(_eventClusterCreated.IsCompleted);
            Assert.AreEqual(_eventClusterCreatedAddedFault.Result.Message.Exceptions[0].Message, "No device found that matches DeviceId: f2a684cc-d2c2-4a25-9257-f8d5e09d2a3f");
        }
    }

    

    [TestFixture]
    public class ShouldSucceedCloseExistingCluster : EventClusterCloseRequestedConsumerFixture
    {
        public override async Task SetUp()
        {
            await base.SetUp();
            await InputQueueSendEndpoint.Send(new EventClusterCloseRequestedEvent()
            {
                EventClusterId = new Guid("ddd6db1a-0a6f-447c-a0ed-dca531c5ab11"),
                ClosureDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(12)
            });
        }

        [Test]
        public async Task TestShouldSucceedCloseExistingCluster()
        {
            await Task.WhenAny(_eventClusterClosed, _eventClusterClosedAddedFault);

            Assert.IsTrue(_eventClusterClosed.IsCompleted);
            Assert.IsFalse(_eventClusterClosedAddedFault.IsCompleted);

            Assert.True(VerifyDebugLog("Retrieved close message for event cluster", _mockLogger));
            Assert.True(VerifyDebugLog("Event closed successfully", _mockLogger));
            var events = await _mockEventRest.Object.GetEventClusters();
            var eventsButton = events.ToList().FindAll(p => p.EventType == "button");
            var eventsSound = events.ToList().FindAll(p => p.EventType == "sound");
            var eventsTest = events.ToList().FindAll(p => p.EventType == "test");
            Assert.AreEqual(eventsButton.Count,2 );
            Assert.AreEqual(eventsSound.Count, 1);
            Assert.AreEqual(eventsTest.Count, 0);
        }
    }

    [TestFixture]
    public class ShouldSucceedCloseExistingClusterEndDateOlder : EventClusterCloseRequestedConsumerFixture
    {
        public override async Task SetUp()
        {
            await base.SetUp();
            await InputQueueSendEndpoint.Send(new EventClusterCloseRequestedEvent()
            {
                EventClusterId = new Guid("ddd6db1a-0a6f-447c-a0ed-dca531c5ab11"),
                ClosureDate = DateTime.UtcNow.AddHours(-24),
                EndDate = DateTime.UtcNow.AddHours(-12)
            });
        }

        [Test]
        public async Task TestShouldSucceedCloseExistingClusterEndDateOlder()
        {
            await Task.WhenAny(_eventClusterClosed, _eventClusterClosedAddedFault);

            Assert.IsTrue(_eventClusterClosed.IsCompleted);
            Assert.IsFalse(_eventClusterClosedAddedFault.IsCompleted);

            Assert.True(VerifyDebugLog("Retrieved close message for event cluster", _mockLogger));
            Assert.True(VerifyDebugLog("Event closed successfully", _mockLogger));
            var events = await _mockEventRest.Object.GetEventClusters();
            var eventsButton = events.ToList().FindAll(p => p.EventType == "button");
            var eventsSound = events.ToList().FindAll(p => p.EventType == "sound");
            var eventsTest = events.ToList().FindAll(p => p.EventType == "test");
            Assert.AreEqual(eventsButton.Count, 1);
            Assert.AreEqual(eventsSound.Count, 1);
            Assert.AreEqual(eventsTest.Count, 0);
        }
    }
}
