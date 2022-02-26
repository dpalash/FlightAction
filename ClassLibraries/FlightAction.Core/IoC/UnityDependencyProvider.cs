using System;
using System.Diagnostics;
using System.IO;
using FlightAction.Services;
using FlightAction.Services.Interfaces;
using Framework.IoC;
using Framework.Utility;
using Framework.Utility.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;
using Unity;
using Unity.Lifetime;

namespace FlightAction.IoC
{
    public class UnityDependencyProvider : IDependencyProvider
    {
        private IUnityContainer _unityContainer;
        private IConfiguration Configuration => _unityContainer.Resolve<IConfiguration>();

        public IUnityContainer RegisterDependencies(IUnityContainer container)
        {
            _unityContainer = container;

            DependencyUtility.SetContainer(container);

            container.RegisterFactory<IConfiguration>(m =>
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    //.SetBasePath(Directory.GetCurrentDirectory())
                    //.AddJsonFile("AppSettings.json", false, true)
                    //.AddJsonFile($"AppSettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
                    //.AddEnvironmentVariables()
                    .Build();

                return configuration;
            }, new ContainerControlledLifetimeManager());


            container.RegisterFactory<ILogger>(m =>
            {
                ILogger log = new LoggerConfiguration()
                     .ReadFrom.Configuration(Configuration)
                     .Enrich.FromLogContext()
                     .WriteTo.File($@"{Directory.GetCurrentDirectory()}\log\log.txt", rollingInterval: RollingInterval.Day)
                     .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                     .CreateLogger();

                Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

                return log;
            }, new ContainerControlledLifetimeManager());

            container.RegisterType<IDirectoryUtility, DirectoryUtility>();
            container.RegisterType<IFileUploadService, FileUploadService>();

            return _unityContainer;
        }
    }
}
