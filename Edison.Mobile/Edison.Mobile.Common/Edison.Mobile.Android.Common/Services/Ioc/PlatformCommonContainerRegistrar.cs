using Android.App;
using Autofac;
using Edison.Mobile.Android.Common.Auth;
using Edison.Mobile.Android.Common.Geolocation;
using Edison.Mobile.Android.Common.Logging;
using Edison.Mobile.Android.Common.Notifications;
using Edison.Mobile.Android.Common.WiFi;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Notifications;
using Edison.Mobile.Common.WiFi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.Mobile.Android.Common.Ioc
{
    public class PlatformCommonContainerRegistrar : IContainerRegistrar
    {
        //       readonly Activity mainActivity;

 //       public PlatformCommonContainerRegistrar(Activity mainActivity)

        public PlatformCommonContainerRegistrar()
        {
 //           this.mainActivity = mainActivity;
        }

        public void Register(ContainerBuilder builder)
        {
 //           builder.RegisterInstance<Activity>(this.mainActivity)
 //               .SingleInstance();

            builder.RegisterType<LocationService>()
                    .As<ILocationService>()
                    .SingleInstance();

            builder.RegisterType<PlatformLogger>()
                    .As<BasePlatformLogger>()
                    .SingleInstance();

            builder.RegisterType<PlatformAuthService>()
                    .As<IPlatformAuthService>()
                    .SingleInstance();

            builder.RegisterType<NotificationService>()
                    .As<INotificationService>()
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
                    SSID = "SSID 3"
                });
            }
        }
    }
}
