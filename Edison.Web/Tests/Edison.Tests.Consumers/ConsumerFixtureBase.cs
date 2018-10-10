using Edison.Core.Common.Models;
using Edison.Core.Interfaces;
using MassTransit.TestFramework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace Edison.Tests.Consumers
{
    public class ConsumerFixtureBase : InMemoryTestFixture
    {
        protected Mock<IEventClusterRestService> _mockEventRest;

        protected void SetupMocks()
        {
            _mockEventRest = new Mock<IEventClusterRestService>(MockBehavior.Strict);

            //Event Rest
            _mockEventRest.Setup(p => p.GetEventCluster(It.IsAny<Guid>())).Returns<Guid>((p) => Task.FromResult(DBMock.GetEventCluster(p)));
            _mockEventRest.Setup(p => p.GetEventClusters()).Returns(() => Task.FromResult(DBMock.GetEventClusters()));
            _mockEventRest.Setup(p => p.CreateOrUpdateEventCluster(It.IsAny<EventClusterCreationModel>())).Returns<EventClusterCreationModel>((p) => Task.FromResult(DBMock.CreateOrUpdateEventCluster(p)));
            _mockEventRest.Setup(p => p.CloseEventCluster(It.IsAny<EventClusterCloseModel>())).Returns<EventClusterCloseModel>((p) => Task.FromResult(DBMock.CloseEventCluster(p)));
        }

        protected Mock<ILogger<T>> CreateLogger<T>()
        {
            var mock = new Mock<ILogger<T>>(MockBehavior.Strict);

            mock.Setup(m => m.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<FormattedLogValues>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>())
                );

            return mock;
        }

        protected bool VerifyDebugLog<T>(string log, Mock<ILogger<T>> mock)
        {
            try
            {
                mock.Verify(m => m.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<FormattedLogValues>(v => v.ToString().Contains(log)),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>())
                );
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected bool VerifyErrorLog<T>(string log, Mock<ILogger<T>> mock)
        {
            try
            {
                mock.Verify(m => m.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<FormattedLogValues>(v => v.ToString().Contains(log)),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>())
                );
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected string CreateDeviceTriggerIoTMessage(IDictionary<string, object> eventMetadata)
        {
            dynamic newObject = new ExpandoObject();
            if (eventMetadata != null)
                newObject.Metadata = eventMetadata;

            return JsonConvert.SerializeObject(newObject);
        }
    }
}
