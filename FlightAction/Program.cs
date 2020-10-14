using FlightAction.IoC;
using Unity;
using DomainExceptionHandler = FlightAction.ExceptionHandling.DomainExceptionHandler;

namespace FlightAction
{
    static class Program
    {
        static void Main(string[] args)
        {
            DomainExceptionHandler.HandleDomainExceptions();
            ConfigureUnityContainer();
        }

        private static void ConfigureUnityContainer()
        {
            var unityDependencyProvider = new UnityDependencyProvider();
            unityDependencyProvider.RegisterDependencies(new UnityContainer());
        }
    }
}
