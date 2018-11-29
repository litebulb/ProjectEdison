using System;
using Autofac;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.Common.Logging;
using Edison.Mobile.Common.Shared;
using Edison.Mobile.User.Client.Core.Chat;
using Edison.Mobile.User.Client.Core.Network;
using Edison.Mobile.User.Client.Core.ViewModels;

namespace Edison.Mobile.User.Client.Core.Ioc
{
    public class CoreContainerRegistrar : IContainerRegistrar
    {
        public void Register(ContainerBuilder builder)
        {
            builder.RegisterType<MainViewModel>();
            builder.RegisterType<ResponsesViewModel>();
            builder.RegisterType<MenuViewModel>();
            builder.RegisterType<ResponseDetailsViewModel>();
            builder.RegisterType<ChatViewModel>().SingleInstance();

            builder.RegisterType<ChatClientConfig>().SingleInstance();


            builder.Register((c, p) => new ResponseRestService(c.Resolve<AuthService>(), c.Resolve<ILogger>(), Constants.BaseUrl));
            builder.Register((c, p) => new EventClusterRestService(c.Resolve<AuthService>(), c.Resolve<ILogger>(), Constants.BaseUrl));
            builder.Register((c, p) => new ActionPlanRestService(c.Resolve<AuthService>(), c.Resolve<ILogger>(), Constants.BaseUrl));

            builder.Register((c, p) => new ChatRestService(c.Resolve<AuthService>(), c.Resolve<ILogger>(), Constants.ChatBaseUrl));
        }
    }
}
