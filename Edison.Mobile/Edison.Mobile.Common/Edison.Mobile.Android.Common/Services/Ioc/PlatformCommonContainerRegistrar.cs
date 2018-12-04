using Android.App;
using Autofac;
using Edison.Mobile.Android.Common.Auth;
using Edison.Mobile.Android.Common.Geolocation;
using Edison.Mobile.Android.Common.Logging;
using Edison.Mobile.Android.Common.Notifications;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Notifications;

namespace Edison.Mobile.Android.Common.Ioc
{
    public class PlatformCommonContainerRegistrar : IContainerRegistrar
    {
        readonly Activity mainActivity;
        public PlatformCommonContainerRegistrar(Activity mainActivity)
        {
            this.mainActivity = mainActivity;
        }

        public void Register(ContainerBuilder builder)
        {
            builder.RegisterInstance<Activity>(this.mainActivity)
                .SingleInstance();

            builder.RegisterType<LocationService>()
                   .As<ILocationService>()
                   .SingleInstance();

            builder.RegisterType<PlatformLogger>()
                   .As<BasePlatformLogger>()
                   .SingleInstance();

            builder.RegisterType<PlatformAuthService>()
                   .As<IPlatformAuthService>();

            builder.RegisterType<NotificationService>()
                   .As<INotificationService>()
                   .SingleInstance();

        }
    }
}
