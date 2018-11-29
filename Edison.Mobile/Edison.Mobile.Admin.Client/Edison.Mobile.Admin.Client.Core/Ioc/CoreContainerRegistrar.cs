using Autofac;
using Edison.Mobile.Admin.Client.Core.ViewModels;
using Edison.Mobile.Common.Ioc;

namespace Edison.Mobile.Admin.Client.Core.Ioc
{
    public class CoreContainerRegistrar : IContainerRegistrar
    {
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<MainViewModel>();
            builder.RegisterType<LoginViewModel>();
        }
    }
}
