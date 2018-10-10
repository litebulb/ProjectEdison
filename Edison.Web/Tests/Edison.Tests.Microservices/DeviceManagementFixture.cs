using Microsoft.Extensions.Logging;
using Moq;

namespace Edison.Tests.Microservices
{
    public class DeviceManagementFixture : MicroServicesFixtureBase
    {
        //protected Mock<ILogger<DeviceManagementService.Program>> _mockLogger;

        public DeviceManagementFixture()
        {
           //_mockLogger = CreateLogger<EventProcessorService.EventProcessorService>();
        }

        /*private EventProcessorService.EventProcessorService CreateDeviceManagementService()
        {
            return
                new EventProcessorService.EventProcessorService(
                    _mockEventRest.Object,
                    _mockAzureServiceBus.Object,
                    _mockServiceBus.Object,
                    _mockLogger.Object);
        }*/
    }
}
