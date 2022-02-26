using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlightAction.IoC;
using FlightAction.Models;
using Flurl.Http;
using Framework.Extensions;
using Microsoft.Extensions.Configuration;
using Unity;

namespace FlightAction.Core
{
    public class InitialSetup
    {
        public InitialSetup()
        {
            
        }

        private static void ConfigureFlurlHttpClient(IUnityContainer unityContainer)
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

        private static void GlobalConfigurationSetup()
        {
            //GlobalConfiguration.Configuration
            //    .UseMemoryStorage()
            //    .UseColouredConsoleLogProvider()
            //    .UseSerilogLogProvider();
        }

        private static IUnityContainer ConfigureUnityContainer()
        {
            var unityDependencyProvider = new UnityDependencyProvider();
            return unityDependencyProvider.RegisterDependencies(new UnityContainer());
        }

    }
}
