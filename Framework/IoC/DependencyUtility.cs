using System;
using Unity;

namespace Framework.IoC
{
    public static class DependencyUtility
    {
        private static IUnityContainer InitializeUnityContainer()
        {
            var container = new UnityContainer();
            Container = container;
            return container;
        }

        public static void SetContainer(IUnityContainer container)
        {
            Container = container;
        }

        public static void Reset()
        {
            Container.Dispose();
            Container = InitializeUnityContainer();
        }

        public static IUnityContainer Container { get; private set; }

        public static T Resolve<T>() => Container.Resolve<T>();

        public static T Resolve<T>(string name) => Container.Resolve<T>(name);

        public static object Resolve(Type t) => Container.Resolve(t);
    }
}
