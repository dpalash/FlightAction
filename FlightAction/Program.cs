using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlightAction.ExceptionHandling;
using FlightAction.IoC;
using Flurl.Http;
using Framework.Extensions;
using Framework.WindowsService;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Unity;
using Unity.Microsoft.DependencyInjection;
using FlightActionAsServiceHost = FlightAction.WindowsService.FlightActionAsServiceHost;

namespace FlightAction
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            DomainExceptionHandler.HandleDomainExceptions();

            GlobalConfigurationSetup();

            var unityContainer = ConfigureUnityContainer();

            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule?.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            var hostBuilder = Host.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUnityServiceProvider(unityContainer)
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<FlightActionAsServiceHost>();
                });

            //INFO: Don't move this method from here.
            ConfigureFlurlHttpClient(unityContainer);

            if (isService)
                await hostBuilder.RunAsServiceAsync();
            else
                await hostBuilder.RunConsoleAsync();
        }

        private static void ConfigureFlurlHttpClient(IUnityContainer unityContainer)
        {
            var configuration = unityContainer.Resolve<IConfiguration>();

            // Do this in Startup. All calls to SimpleCast will use the same HttpClient instance.
            FlurlHttp.ConfigureClient(configuration["ServerHost"], cli => cli
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
        }

        private static void GlobalConfigurationSetup()
        {
            GlobalConfiguration.Configuration
                .UseMemoryStorage()
                .UseColouredConsoleLogProvider()
                .UseSerilogLogProvider();
        }

        private static IUnityContainer ConfigureUnityContainer()
        {
            var unityDependencyProvider = new UnityDependencyProvider();
            return unityDependencyProvider.RegisterDependencies(new UnityContainer());
        }
    }
}
