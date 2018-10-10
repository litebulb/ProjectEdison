using System;
using Autofac;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.User.Client.Core.ViewModels;

namespace Edison.Mobile.User.Client.Core.Ioc
{
    public class CoreContainerRegistrar : IContainerRegistrar
    {
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<MainViewModel>();
            builder.RegisterType<LoginViewModel>();
            builder.RegisterType<ResponsesViewModel>();
            builder.RegisterType<MenuViewModel>();
            builder.RegisterType<ResponseDetailsViewModel>();
        }
    }
}
