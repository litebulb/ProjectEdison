using System;
using Autofac;

namespace Edison.Mobile.Common.Ioc
{
    public static class Container
    {
        static IContainer instance;

        public static IContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new NullReferenceException("IOC Container not initialized.");
                }

                return instance;
            }
        }

        public static void Initialize(params IContainerRegistrar[] registrars)
        {
            var builder = new ContainerBuilder();

            CommonContainerRegistrar.Register(builder);

            foreach (var registrar in registrars)
            {
                registrar.Register(builder);
            }

            instance = builder.Build();
        }
    }
}
