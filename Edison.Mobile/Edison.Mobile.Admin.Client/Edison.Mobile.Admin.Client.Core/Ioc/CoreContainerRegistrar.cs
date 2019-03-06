using Autofac;
using Edison.Mobile.Admin.Client.Core.Auth;
using Edison.Mobile.Admin.Client.Core.Network;
using Edison.Mobile.Admin.Client.Core.Services;
using Edison.Mobile.Admin.Client.Core.Shared;
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
            builder.RegisterType<LoginViewModel>();
            builder.RegisterType<ChooseDeviceTypeViewModel>();
            builder.RegisterType<SelectWifiViewModel>();
            builder.RegisterType<RegisterDeviceViewModel>();
            builder.RegisterType<ManageDeviceViewModel>();
            builder.RegisterType<EnterWifiPasswordViewModel>();
            builder.RegisterType<CompleteRegistrationViewModel>();

            builder.RegisterType<AppAuthService>()
                   .As<IAppAuthService>();

            builder.Register<IDeviceRestService>((c, p) => new DeviceRestService(c.Resolve<AuthService>(), c.Resolve<ILogger>(), Constants.BaseUrl));

            builder.Register<IOnboardingRestService>((c, p) => new OnboardingRestService(c.Resolve<AuthService>(), c.Resolve<ILogger>(), DeviceConfig.BaseUrl));

            builder.Register((c, p) => new DeviceProvisioningRestService(c.Resolve<AuthService>(), c.Resolve<ILogger>(), DeviceConfig.ProvisioningBaseUrl));

            builder.RegisterType<DeviceSetupService>()
                   .SingleInstance();

            builder.RegisterType<MainViewModel>();


#if ANDROIDADMINNOPI
            ServiceMocks.Setup(builder);
#endif

        }
    }
}
