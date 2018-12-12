using Autofac;
using Edison.Mobile.Admin.Client.Core.Auth;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Shared;

namespace Edison.Mobile.Admin.Client.Core.Ioc
{
    public class CoreContainerRegistrar : IContainerRegistrar
    {
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<MainViewModel>();
            builder.RegisterType<LoginViewModel>();
            builder.RegisterType<ChooseDeviceTypeViewModel>();

            builder.RegisterType<AppAuthService>()
                   .As<IAppAuthService>();

            builder.Register((c, p) => new DeviceRestService(c.Resolve<AuthService>(), c.Resolve<ILogger>(), Constants.BaseUrl));
        }
    }
}
