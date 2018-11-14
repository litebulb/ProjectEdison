using System;
using Autofac;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Notifications;
using Edison.Mobile.iOS.Common.Auth;
using Edison.Mobile.iOS.Common.LocationServices;
using Edison.Mobile.iOS.Common.Logging;
using Edison.Mobile.iOS.Common.Notifications;

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
                    .As<IPlatformAuthService>();
        }
    }
}
