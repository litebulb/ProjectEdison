using Autofac;

namespace Edison.Mobile.Common.Ioc
{
    public interface IContainerRegistrar
    {
        void Register(ContainerBuilder builder);
    }
}
