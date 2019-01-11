using System;
using Microsoft.Practices.Unity;

namespace GazRouter.Application.ModuleManagement
{
    public static class UnityContainerExtensions
    {
        public static IUnityContainer RegisterFactory<TI, T>(this IUnityContainer container, string name, Func<T> factoryMethod) where T : TI
        {
            return container.RegisterType<TI, T>(name, new FactoryLifetimeManager<T>(factoryMethod, new TransientLifetimeManager()));
        }


        public static IUnityContainer RegisterFactory<T>(this IUnityContainer container, Func<T> factoryMethod)
        {
            return container.RegisterFactory(factoryMethod, new TransientLifetimeManager());
        }

        public static IUnityContainer RegisterFactory<T>(this IUnityContainer container, Func<T> factoryMethod, LifetimeManager lifetimeManager)
        {
            return container.RegisterType<T>(new FactoryLifetimeManager<T>(factoryMethod, lifetimeManager));
        }
    }
}