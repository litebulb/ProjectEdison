using Autofac;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Network;

namespace Edison.Mobile.Common.Ioc
{
    public static class CommonContainerRegistrar
    {
        public static void Register(ContainerBuilder builder)
        {

            builder.RegisterType<AuthService>().SingleInstance();

            builder.RegisterType<CommonLogger>().As<ILogger>();

            var baseUrl = "https://edisonapidev.eastus.cloudapp.azure.com/api/";
            builder.Register((c, p) => new ResponseRestService(c.Resolve<AuthService>(), c.Resolve<ILogger>(), baseUrl));
            builder.Register((c, p) => new EventClusterRestService(c.Resolve<AuthService>(), c.Resolve<ILogger>(), baseUrl));
            builder.Register((c, p) => new ActionPlanRestService(c.Resolve<AuthService>(), c.Resolve<ILogger>(), baseUrl));
        }
    }
}
