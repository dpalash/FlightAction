using FlightAction.ExceptionHandling;
using Framework.WindowsService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlightAction.Core;
using Unity.Microsoft.DependencyInjection;
using FlightActionAsServiceHost = FlightAction.WindowsService.FlightActionAsServiceHost;

namespace FlightAction
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            DomainExceptionHandler.HandleDomainExceptions();

            InitialSetup.GlobalConfigurationSetup();

            var unityContainer = InitialSetup.ConfigureUnityContainer();

            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule?.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(config =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var env = context.HostingEnvironment;
                    builder.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"AppSettings.{env.EnvironmentName}.json", true, true)
                        .AddEnvironmentVariables();
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUnityServiceProvider(unityContainer)
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<FlightActionAsServiceHost>();
                    services.AddHttpClient();
                });

            //INFO: Don't move this method from here.
            InitialSetup.ConfigureFlurlHttpClient(unityContainer);

            if (isService)
                await hostBuilder.RunAsServiceAsync();
            else
                await hostBuilder.RunConsoleAsync();
        }
    }
}
