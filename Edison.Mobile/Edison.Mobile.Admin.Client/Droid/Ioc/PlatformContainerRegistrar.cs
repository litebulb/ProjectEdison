using Autofac;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Ioc;
using Microsoft.Identity.Client;
using Edison.Mobile.Admin.Client.Droid.Shared;

namespace Edison.Mobile.Admin.Client.Droid.Ioc
{
    public class PlatformContainerRegistrar : IContainerRegistrar
    {
        public void Register(ContainerBuilder builder)
        {
            var publicClientApplication = new PublicClientApplication(Constants.ClientId, AuthConfig.AdminAuthority)
            {
                RedirectUri = AuthConfig.AndroidAdminRedirectUri,
            };
                       
            builder.RegisterInstance(publicClientApplication)
                   .As<IPublicClientApplication>();
        }
    }
}
