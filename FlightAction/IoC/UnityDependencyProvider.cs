using System;
using System.Diagnostics;
using System.IO;
using FlightAction.Services;
using FlightAction.Services.Interfaces;
using Flurl.Http;
using Framework.Extensions;
using Framework.IoC;
using Microsoft.Extensions.Configuration;
using Serilog;
using Unity;
using Unity.Lifetime;

namespace FlightAction.IoC
{
    public class UnityDependencyProvider : IDependencyProvider
    {
        private IUnityContainer _container;
        private IConfiguration Configuration => _container.Resolve<IConfiguration>();

        public void RegisterDependencies(IUnityContainer container)
        {
            _container = container;

            DependencyUtility.SetContainer(container);

            container.RegisterFactory<IConfiguration>(m =>
            {
                IConfiguration configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("AppSettings.json", false, true)
                   .AddJsonFile($"AppSettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
                   .AddEnvironmentVariables()
                   .Build();

                return configuration;
            }, new SingletonLifetimeManager());

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

            // Do this in Startup. All calls to SimpleCast will use the same HttpClient instance.
            FlurlHttp.ConfigureClient(Configuration["ServerHost"], cli => cli
                .Configure(settings =>
                {
                    // keeps logging & error handling out of SimpleCastClient
                    settings.BeforeCall = call => Framework.Logger.Log.Logger.Information($"Calling: {call.Request.RequestUri}");
                    settings.AfterCall = call => Framework.Logger.Log.Logger.Information($"Execution completed: {call.Request.RequestUri}");
                    settings.OnError = call => Framework.Logger.Log.Logger.Fatal(call.Exception, call.Exception.GetExceptionDetailMessage());
                })
                // adds default headers to send with every call
                .WithHeaders(new
                {
                    Accept = "application/json",
                    User_Agent = "MyCustomUserAgent" // Flurl will convert that underscore to a hyphen
                }));

            container.RegisterType<IFileUploadService, FileUploadService>();
        }
    }
}
