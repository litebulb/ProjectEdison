using Autofac;
using Edison.Mobile.Android.Common.Auth;
using Edison.Mobile.Android.Common.Geolocation;
using Edison.Mobile.Android.Common.Logging;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Geolocation;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.Logging;

namespace Edison.Mobile.Android.Common.Ioc
{
    public class PlatformCommonContainerRegistrar : IContainerRegistrar
    {
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<LocationService>()
                   .As<ILocationService>()
                   .SingleInstance();

            builder.RegisterType<PlatformLogger>()
                   .As<BasePlatformLogger>()
                   .SingleInstance();

            builder.RegisterType<PlatformAuthService>()
                   .As<IPlatformAuthService>();
        }
    }
}
