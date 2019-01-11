using Microsoft.Practices.Unity;

namespace GazRouter.Application.Helpers
{
    public static class UnityContainerExtensions
    {
        public static void RegisterSingletonType<T>(this IUnityContainer container)
        {
        
            container.RegisterType<T>(new ContainerControlledLifetimeManager());
        }

        public static void RegisterSingletonType<TFrom, TTo>(this IUnityContainer container) where TTo : TFrom
        {
            container.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
        }

        public static void RegisterSingletonType<T>(this IUnityContainer container, string param)
        {
            container.RegisterType<T>(param, new ContainerControlledLifetimeManager(), new InjectionConstructor(param));
        }
    }
}