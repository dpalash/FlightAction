using FlightAction.Core.IoC;
using FlightAction.Core.Models;
using Flurl.Http;
using Framework.Extensions;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Unity;

namespace FlightAction.Core
{
    public static class InitialSetup
    {
        public static void ConfigureFlurlHttpClient(IUnityContainer unityContainer)
        {
            var configuration = unityContainer.Resolve<IConfiguration>();

            // Do this in Startup. All calls to SimpleCast will use the same HttpClient instance.
            FlurlHttp.ConfigureClient(configuration["ServerHost"], cli => cli
                .Configure(settings =>
                {
                    settings.HttpClientFactory = new UntrustedCertClientFactory();

                    // keeps logging & error handling out of SimpleCastClient
                    settings.BeforeCall = call => Framework.Logger.Log.Logger.Information($"Calling: {call.Request.Url.Path}");
                    settings.AfterCall = call => Framework.Logger.Log.Logger.Information($"Execution completed: {call.Request.Url.Path}");
                    settings.OnError = call => Framework.Logger.Log.Logger.Fatal(call.Exception, call.Exception.GetExceptionDetailMessage());
                })
                // adds default headers to send with every call
                .WithHeaders(new
                {
                    Accept = "application/json",
                    User_Agent = "MyCustomUserAgent" // Flurl will convert that underscore to a hyphen
                }));
        }

        public static void GlobalConfigurationSetup()
        {
            GlobalConfiguration.Configuration
                .UseMemoryStorage()
                .UseColouredConsoleLogProvider()
                .UseSerilogLogProvider();
        }

        public static IUnityContainer ConfigureUnityContainer()
        {
            var unityDependencyProvider = new UnityDependencyProvider();
            return unityDependencyProvider.RegisterDependencies(new UnityContainer());
        }

    }
}
