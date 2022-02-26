using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AutoCount
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //DomainExceptionHandler.HandleDomainExceptions();

            //GlobalConfigurationSetup();

            //var unityContainer = ConfigureUnityContainer();

            //var isService = !(Debugger.IsAttached || args.Contains("--console"));

            //if (isService)
            //{
            //    var pathToExe = Process.GetCurrentProcess().MainModule?.FileName;
            //    var pathToContentRoot = Path.GetDirectoryName(pathToExe);
            //    Directory.SetCurrentDirectory(pathToContentRoot);
            //}

            //var hostBuilder = Host.CreateDefaultBuilder(args)
            //    .ConfigureHostConfiguration(config =>
            //    {
            //        config.AddEnvironmentVariables();
            //    })
            //    .ConfigureAppConfiguration((context, builder) =>
            //    {
            //        var env = context.HostingEnvironment;
            //        builder.SetBasePath(Directory.GetCurrentDirectory())
            //            .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true)
            //            .AddJsonFile($"AppSettings.{env.EnvironmentName}.json", true, true)
            //            .AddEnvironmentVariables();
            //    })
            //    .UseContentRoot(Directory.GetCurrentDirectory())
            //    .UseUnityServiceProvider(unityContainer)
            //    .UseSerilog()
            //    .ConfigureServices((hostContext, services) =>
            //    {
            //        services.AddHostedService<FlightActionAsServiceHost>();
            //        services.AddHttpClient();
            //    });

            ////INFO: Don't move this method from here.
            //ConfigureFlurlHttpClient(unityContainer);

            //if (isService)
            //    await hostBuilder.RunAsServiceAsync();
            //else
            //    await hostBuilder.RunConsoleAsync();
        }

        protected override void OnStop()
        {
        }
    }
}
