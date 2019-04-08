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
            
            builder.RegisterInstance<IWifiService>(new WifiServiceMock());
#endif
        }


        public class WifiServiceMock : IWifiService
        {

            public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed;
            public event EventHandler<CheckingConnectionStatusUpdatedEventArgs> CheckingConnectionStatusUpdated;

            public Task<bool> ConnectToWifiNetwork(string ssid)
            {
                return Task.FromResult(true);
            }

            public Task<bool> ConnectToWifiNetwork(string ssid, string passphrase)
            {
                return Task.FromResult(true);
            }

            public Task DisconnectFromWifiNetwork(WifiNetwork wifiNetwork)
            {
                return Task.FromResult(true);
            }

            public Task<WifiNetwork> GetCurrentlyConnectedWifiNetwork()
            {
                return Task.FromResult(new WifiNetwork()
                {
                    SSID = "SSID 1"
                });
            }
        }
    }
}
