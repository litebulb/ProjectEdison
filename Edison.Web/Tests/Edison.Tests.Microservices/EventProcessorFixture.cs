using Edison.Common.Interfaces;
using Edison.Common.Messages.IoTMessages;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace Edison.Tests.Microservices
{
    public class EventProcessorFixture : MicroServicesFixtureBase
    {
        protected Mock<IAzureServiceBusClient<DeviceTriggerIoTMessage>> _mockAzureServiceBus;
        protected Mock<ILogger<EventProcessorService.EventProcessorService>> _mockLogger;

        public EventProcessorFixture()
        {
            _mockAzureServiceBus = new Mock<IAzureServiceBusClient<DeviceTriggerIoTMessage>>(MockBehavior.Strict);
            _mockLogger = CreateLogger<EventProcessorService.EventProcessorService>();
        }

        private EventProcessorService.EventProcessorService CreateEventProcessorService()
        {
            return
                new EventProcessorService.EventProcessorService(
                    _mockEventRest.Object,
                    _mockAzureServiceBus.Object,
                    _mockServiceBus.Object,
                    _mockLogger.Object);
        }

        private EventProcessorService.EventProcessorService CreateEventProcessorServiceWithFailingRest()
        {
            return
                new EventProcessorService.EventProcessorService(
                    _mockEventRestFailing.Object,
                    _mockAzureServiceBus.Object,
                    _mockServiceBus.Object,
                    _mockLogger.Object);
        }

        [Fact]
        public async void SendNewIoTButtonMessageExists()
        {
            DBMock.Init();
            var service = CreateEventProcessorService();
            await service.HandleEventTriggerInput(CreateDeviceTriggerIoTMessage("DeviceButton3", "button"));

            Assert.True(VerifyDebugLog("Retrieved message", _mockLogger));
            var events = await _mockEventRest.Object.GetEventClusters();
            var eventsButton = events.ToList().FindAll(p => p.EventType == "button");
            var eventsSound = events.ToList().FindAll(p => p.EventType == "sound");
            var eventsTest = events.ToList().FindAll(p => p.EventType == "test");
            Assert.Equal(2, eventsButton.Count);
            Assert.Single(eventsSound);
            Assert.Empty(eventsTest);

            var k = await _mockEventRest.Object.GetEventCluster(eventsButton[0].EventClusterId);

            await service.HandleEventTriggerInput(CreateDeviceTriggerIoTMessage("DeviceButton2", "button"));
            events = await _mockEventRest.Object.GetEventClusters();
            eventsButton = events.ToList().FindAll(p => p.EventType == "button");
            Assert.Equal(3, eventsButton.Count);
        }

        [Fact]
        public async void TestEventRestError()
        {
            DBMock.Init();
            var service = CreateEventProcessorServiceWithFailingRest();
            await service.HandleEventTriggerInput(CreateDeviceTriggerIoTMessage("DeviceButton", "button"));
            Assert.True(VerifyErrorLog("CreateEvent: The event was not created", _mockLogger));
            Assert.True(VerifyErrorLog("HandleEventTriggerInput: The event could not be created.", _mockLogger));
        }
    }
}
