using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Notifications;
using Edison.Mobile.Common.WiFi;
using Edison.Mobile.iOS.Common.Auth;
using Edison.Mobile.iOS.Common.LocationServices;
using Edison.Mobile.iOS.Common.Logging;
using Edison.Mobile.iOS.Common.Notifications;
using Edison.Mobile.iOS.Common.WiFi;
using Moq;

namespace Edison.Mobile.iOS.Common.Ioc
{
    public class PlatformCommonContainerRegistrar : IContainerRegistrar
    {
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<LocationService>()
                   .As<ILocationService>()
                   .SingleInstance();

            builder.RegisterType<NotificationService>()
                   .As<INotificationService>()
                   .SingleInstance();

            builder.RegisterType<PlatformLogger>()
                   .As<BasePlatformLogger>()
                   .SingleInstance();

            builder.RegisterType<PlatformAuthService>()
                   .As<IPlatformAuthService>()
                   .SingleInstance();

            builder.RegisterType<PlatformWifiService>()
                   .As<IWifiService>()
                   .SingleInstance();

#if ANDROIDADMINNOPI
            var WifiServiceMock = new Mock<IWifiService>();
            WifiServiceMock.Setup(i => i.GetCurrentlyConnectedWifiNetwork()).Returns(Task.FromResult(PlatformCommonContainerRegistrar.MockNetworks().First()));
            WifiServiceMock.Setup(i => i.ConnectToWifiNetwork(It.IsAny<string>())).Returns(Task.FromResult(true));
            WifiServiceMock.Setup(i => i.ConnectToWifiNetwork(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            WifiServiceMock.Setup(i => i.DisconnectFromWifiNetwork(It.IsAny<WifiNetwork>())).Returns(Task.FromResult(true));

            builder.RegisterInstance<IWifiService>(WifiServiceMock.Object);
#endif
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
