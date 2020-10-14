using System;
using System.IO;
using Framework.IoC;
using Microsoft.Extensions.Configuration;
using Serilog;
using Unity;
using Unity.Lifetime;

namespace FlightAction.IoC
{
    public class UnityDependencyProvider : IDependencyProvider
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSettings.json", false, true)
            .AddJsonFile($"AppSettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
            .AddEnvironmentVariables()
            .Build();

        public void RegisterDependencies(IUnityContainer container)
        {
            DependencyUtility.SetContainer(container);

            container.RegisterFactory<ILogger>(m =>
           {
               ILogger log = new LoggerConfiguration()
                   .ReadFrom.Configuration(Configuration)
                   .Enrich.FromLogContext()
                   .WriteTo.File($@"{Directory.GetCurrentDirectory()}\log\log.txt", rollingInterval: RollingInterval.Day)
                   .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                   .CreateLogger();

               return log;
           }, new ContainerControlledLifetimeManager());
        }
    }
}
