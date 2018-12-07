using System;
using Autofac;
using Edison.Mobile.Admin.Client.iOS.Shared;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Ioc;
using Microsoft.Identity.Client;

namespace Edison.Mobile.Admin.Client.iOS.Ioc
{
    public class PlatformContainerRegistrar : IContainerRegistrar
    {
        public void Register(ContainerBuilder builder)
        {
            var publicClientApplication = new PublicClientApplication(Constants.ClientId, AuthConfig.AdminAuthority)
            {
                RedirectUri = AuthConfig.AdminRedirectUri,
            };

            builder.RegisterInstance(publicClientApplication)
                   .As<IPublicClientApplication>();
        }
    }
}
