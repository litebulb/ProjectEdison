using Edison.Mobile.Admin.Client.Core.Network;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Edison.Mobile.Common.WiFi;
using System.Threading.Tasks;
using System.Linq;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Autofac;
using Edison.Mobile.Admin.Client.Core.Models;

namespace Edison.Mobile.Admin.Client.Core.Ioc
{
    public class ServiceMocks
    {
        public static Mock<IOnboardingRestService> OnboardingServiceMock;
        public static Mock<IWifiService> WifiServiceMock;

        public static void Setup(ContainerBuilder builder)
        {
            OnboardingServiceMock = new Mock<IOnboardingRestService>();

            OnboardingServiceMock.Setup(i => i.GetAvailableWifiNetworks()).Returns(Task.FromResult(ServiceMocks.MockNetworks()));
            OnboardingServiceMock.Setup(i => i.ConnectToNetwork(It.IsAny<RequestNetworkInformationModel>())).Returns(Task.FromResult(
                new ResultCommandNetworkStatus() { IsSuccess = true, Status = "Connected" }
                ));

            builder.RegisterInstance<IOnboardingRestService>(OnboardingServiceMock.Object);

        }

        public static IEnumerable<WifiNetwork> MockNetworks()
        {
            return new List<WifiNetwork>()
            {
                new WifiNetwork(){ SSID = "SSID 1"},
                new WifiNetwork(){ SSID = "SSID 2"},
            };
        }

    }
}