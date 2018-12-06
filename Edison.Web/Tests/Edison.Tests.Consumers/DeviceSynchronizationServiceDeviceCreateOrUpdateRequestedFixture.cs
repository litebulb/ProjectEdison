using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using Edison.Core.Interfaces;
using Edison.DeviceSynchronizationService.Consumers;
using Edison.Tests.Common.Helpers;
using MassTransit;
using MassTransit.TestFramework;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Edison.Tests.Consumers
{
    #pragma warning disable 1998
    public class DeviceCreateOrUpdateRequestedConsumerFixture : InMemoryTestFixture
    {
        protected Mock<ILogger<DeviceCreateOrUpdateRequestedConsumer>> _mockLogger;
        protected Task<ConsumeContext<IDeviceCreatedOrUpdated>> _deviceCreatedOrUpdated;
        protected Task<ConsumeContext<Fault<IDeviceCreateOrUpdateRequested>>> _deviceCreateOrUpdateFault;
        protected Mock<IDeviceRestService> _mockDevicesRest;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _mockDevicesRest = new Mock<IDeviceRestService>(MockBehavior.Strict);
            configurator.Consumer(() => new DeviceCreateOrUpdateRequestedConsumer(_mockDevicesRest.Object, _mockLogger.Object));
            _deviceCreatedOrUpdated = Handled<IDeviceCreatedOrUpdated>(configurator);
            _deviceCreateOrUpdateFault = Handled<Fault<IDeviceCreateOrUpdateRequested>>(configurator);

            //Mock
            _mockDevicesRest.Setup(p => p.UpdateHeartbeat(It.IsAny<Guid>())).Returns<Guid>((p) => Task.FromResult(MockUpdateHeartbeat(p)));
            _mockDevicesRest.Setup(p => p.CreateOrUpdateDevice(It.IsAny<DeviceTwinModel>())).Returns<DeviceTwinModel>((p) => Task.FromResult(MockCreateOrUpdateDevice(p)));
        }

        [OneTimeSetUp]
        public virtual async Task SetUp()
        {
            _mockLogger = LoggerHelper.CreateLogger<DeviceCreateOrUpdateRequestedConsumer>();
        }

        protected DeviceHeartbeatUpdatedModel MockUpdateHeartbeat(Guid deviceId)
        {
            if (deviceId == Guid.Empty)
                return null;

            return new DeviceHeartbeatUpdatedModel()
            {
                 Device = new DeviceModel(), NeedsUpdate = true
            };
        }

        protected DeviceModel MockCreateOrUpdateDevice(DeviceTwinModel model)
        {
            if (model == null)
                return null;

            if (model.DeviceId == Guid.Empty)
                return new DeviceModel() { DeviceId = Guid.Empty };

            return new DeviceModel()
            {
                 DeviceId = model.DeviceId
            };
        }
    }

    [TestFixture]
    public class ShouldSucceedCreateOrUpdatePingDevice : DeviceCreateOrUpdateRequestedConsumerFixture
    {
        public override async Task SetUp()
        {
            await base.SetUp();
            await InputQueueSendEndpoint.Send(new DeviceCreateOrUpdateRequestedEvent()
            {
                DeviceId = new Guid("fbc64b5c-ff21-4ade-9440-85f7b16ef01e"),
                ChangeType = "ping",
                CorrelationId = Guid.NewGuid(), 
                Date = DateTime.UtcNow,
                Data = "{}"
            });
        }

        [Test]
        public async Task TestShouldSucceedCreateOrUpdatePingDevice()
        {
            await Task.WhenAny(_deviceCreatedOrUpdated, _deviceCreateOrUpdateFault);

            Assert.IsTrue(_deviceCreatedOrUpdated.IsCompleted);
            Assert.IsFalse(_deviceCreateOrUpdateFault.IsCompleted);

            Assert.True(LoggerHelper.VerifyDebugLog("DeviceCreateOrUpdateRequestedConsumer: Retrieved message from device 'fbc64b5c-ff21-4ade-9440-85f7b16ef01e'", _mockLogger));
            Assert.True(LoggerHelper.VerifyDebugLog("DeviceCreateOrUpdateRequestedConsumer: Device heartbeat updated with device id", _mockLogger));
        }
    }

    [TestFixture]
    public class ShouldFailCreateOrUpdatePingDevice : DeviceCreateOrUpdateRequestedConsumerFixture
    {
        public override async Task SetUp()
        {
            await base.SetUp();
            await InputQueueSendEndpoint.Send(new DeviceCreateOrUpdateRequestedEvent()
            {
                DeviceId = Guid.Empty,
                ChangeType = "ping",
                CorrelationId = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Data = "{}"
            });
        }

        [Test]
        public async Task TestShouldFailCreateOrUpdatePingDevice()
        {
            await Task.WhenAny(_deviceCreatedOrUpdated, _deviceCreateOrUpdateFault);

            Assert.IsFalse(_deviceCreatedOrUpdated.IsCompleted);
            Assert.IsTrue(_deviceCreateOrUpdateFault.IsCompleted);

            Assert.True(LoggerHelper.VerifyDebugLog("DeviceCreateOrUpdateRequestedConsumer: Retrieved message from device '", _mockLogger));
            Assert.True(LoggerHelper.VerifyErrorLog("DeviceCreateOrUpdateRequestedConsumer: The device heartbeat could not be updated.", _mockLogger));
        }
    }

    [TestFixture]
    public class ShouldSucceedCreateOrUpdateTwinDevice : DeviceCreateOrUpdateRequestedConsumerFixture
    {
        public override async Task SetUp()
        {
            await base.SetUp();
            await InputQueueSendEndpoint.Send(new DeviceCreateOrUpdateRequestedEvent()
            {
                DeviceId = new Guid("fbc64b5c-ff21-4ade-9440-85f7b16ef01e"),
                ChangeType = "twinChangeNotification",
                CorrelationId = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Data = "{}"
            });
        }

        [Test]
        public async Task TestShouldSucceedCreateOrUpdateTwinDevice()
        {
            await Task.WhenAny(_deviceCreatedOrUpdated, _deviceCreateOrUpdateFault);

            Assert.IsTrue(_deviceCreatedOrUpdated.IsCompleted);
            Assert.IsFalse(_deviceCreateOrUpdateFault.IsCompleted);

            Assert.True(LoggerHelper.VerifyDebugLog("DeviceCreateOrUpdateRequestedConsumer: Retrieved message from device 'fbc64b5c-ff21-4ade-9440-85f7b16ef01e'", _mockLogger));
            Assert.True(LoggerHelper.VerifyDebugLog("DeviceCreateOrUpdateRequestedConsumer: Device created/updated with device id", _mockLogger));
        }
    }

    [TestFixture]
    public class ShouldFailCreateOrUpdateTwinDevice : DeviceCreateOrUpdateRequestedConsumerFixture
    {
        public override async Task SetUp()
        {
            await base.SetUp();
            await InputQueueSendEndpoint.Send(new DeviceCreateOrUpdateRequestedEvent()
            {
                DeviceId = Guid.Empty,
                ChangeType = "twinChangeNotification",
                CorrelationId = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Data = "{}"
            });
        }

        [Test]
        public async Task TestShouldFailCreateOrUpdateTwinDevice()
        {
            await Task.WhenAny(_deviceCreatedOrUpdated, _deviceCreateOrUpdateFault);

            Assert.IsFalse(_deviceCreatedOrUpdated.IsCompleted);
            Assert.IsTrue(_deviceCreateOrUpdateFault.IsCompleted);

            Assert.True(LoggerHelper.VerifyDebugLog("DeviceCreateOrUpdateRequestedConsumer: Retrieved message from device '", _mockLogger));
            Assert.True(LoggerHelper.VerifyErrorLog("DeviceCreateOrUpdateRequestedConsumer: The device could not be updated.", _mockLogger));
        }
    }
}
