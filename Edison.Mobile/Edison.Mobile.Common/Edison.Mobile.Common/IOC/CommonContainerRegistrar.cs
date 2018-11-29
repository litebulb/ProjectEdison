using Autofac;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Network;
using Edison.Mobile.Common.Shared;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.Common.Ioc
{
    public static class CommonContainerRegistrar
    {
        public static void Register(ContainerBuilder builder)
        {
            builder.RegisterType<AuthService>().SingleInstance();

            builder.RegisterType<CommonLogger>().As<ILogger>();
            builder.Register((c, p) => new NotificationRestService(c.Resolve<AuthService>(), c.Resolve<ILogger>(), Constants.BaseUrl));
            builder.Register((c, p) => new LocationRestService(c.Resolve<AuthService>(), c.Resolve<ILogger>(), Constants.BaseUrl));
        }
    }
}
