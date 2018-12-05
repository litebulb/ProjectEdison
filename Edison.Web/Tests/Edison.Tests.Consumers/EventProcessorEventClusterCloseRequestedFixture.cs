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
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Tests.Consumers
{
    #pragma warning disable 1998
    public class EventClusterCloseRequestedConsumerFixture : InMemoryTestFixture
    {
        protected Mock<ILogger<EventClusterCloseRequestedConsumer>> _mockLogger;
        protected Task<ConsumeContext<IEventClusterClosed>> _eventClusterClosed;
        protected Task<ConsumeContext<Fault<IEventClusterCloseRequested>>> _eventClusterClosedAddedFault;
        protected Mock<IEventClusterRestService> _mockEventRest;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _mockEventRest = new Mock<IEventClusterRestService>(MockBehavior.Strict);
            configurator.Consumer(() => new EventClusterCloseRequestedConsumer(_mockEventRest.Object, _mockLogger.Object));
            _eventClusterClosed = Handled<IEventClusterClosed>(configurator);
            _eventClusterClosedAddedFault = Handled<Fault<IEventClusterCloseRequested>>(configurator);

            //Mock
            _mockEventRest.Setup(p => p.CloseEventCluster(It.IsAny<EventClusterCloseModel>())).Returns<EventClusterCloseModel>((p) => Task.FromResult(MockEventClusterClose(p)));
        }

        [OneTimeSetUp]
        public virtual async Task SetUp()
        {
            _mockLogger = LoggerHelper.CreateLogger<EventClusterCloseRequestedConsumer>();
        }

        protected EventClusterModel MockEventClusterClose(EventClusterCloseModel model)
        {
            if (model == null)
                return null;

            if (model.EventClusterId == Guid.Empty)
                return new EventClusterModel();

            return new EventClusterModel()
            {
                EventClusterId = model.EventClusterId != Guid.Empty ? model.EventClusterId : Guid.Empty
            };
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
                EventClusterId = new Guid("ddd6db1a-0a6f-447c-a0ed-dca531c5ab11")
            });
        }

        [Test]
        public async Task TestShouldSucceedCloseExistingCluster()
        {
            await Task.WhenAny(_eventClusterClosed, _eventClusterClosedAddedFault);

            Assert.IsTrue(_eventClusterClosed.IsCompleted);
            Assert.IsFalse(_eventClusterClosedAddedFault.IsCompleted);

            Assert.True(LoggerHelper.VerifyDebugLog("Retrieved close message for event cluster 'ddd6db1a-0a6f-447c-a0ed-dca531c5ab11'", _mockLogger));
            Assert.True(LoggerHelper.VerifyDebugLog("Event closed successfully", _mockLogger));
        }
    }

    [TestFixture]
    public class ShouldFailCloseEmpty : EventClusterCloseRequestedConsumerFixture
    {
        public override async Task SetUp()
        {
            await base.SetUp();
            await InputQueueSendEndpoint.Send(new EventClusterCloseRequestedEvent()
            {
                EventClusterId = Guid.Empty
            });
        }

        [Test]
        public async Task TestShouldFailCloseEmpty()
        {
            await Task.WhenAny(_eventClusterClosed, _eventClusterClosedAddedFault);

            Assert.IsFalse(_eventClusterClosed.IsCompleted);
            Assert.IsTrue(_eventClusterClosedAddedFault.IsCompleted);

            Assert.True(LoggerHelper.VerifyErrorLog("The event cluster could not be closed", _mockLogger));
        }
    }
}
