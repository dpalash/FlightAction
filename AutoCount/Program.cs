using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Debugging;

namespace AutoCount
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
        }

        //public static void AddLogging(this IServiceCollection services, Microsoft.Extensions.Logging.LogLevel logLevel)
        //{
        //    if (!environment.IsDevelopment())
        //    {
        //        var connectionString = configuration["Logging:LogStorageConnectionString"];
        //        var containerName = configuration["Logging:LogContainerName"];

        //        var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
        //        telemetryConfiguration.InstrumentationKey = configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];

        //        Log.Logger = new Serilog.LoggerConfiguration()
        //            .WriteTo.AzureBlobStorage(connectionString, Serilog.Events.LogEventLevel.Verbose, containerName, "{yyyy}/{MM}/{dd}/log.txt")
        //            .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces)
        //            .MinimumLevel.Debug()
        //            .CreateLogger();
        //    }

        //    else
        //    {
        //        Log.Logger = new LoggerConfiguration()
                    
        //            .WriteTo.File($@"{Directory.GetCurrentDirectory()}\log\log.txt", rollingInterval: RollingInterval.Day)
        //            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        //            .MinimumLevel.Debug()
        //            .CreateLogger();
        //    }

        //    SelfLog.Enable(msg => Debug.WriteLine(msg));

        //    services.AddSingleton(Serilog.Log.Logger);
        //    services.AddSingleton<Infrastructure.Logging.Contracts.ILogger, SerilogLogger>();
        //}
    }
}
