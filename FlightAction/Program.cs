using FlightAction.IoC;
using FlightAction.Services.Interfaces;
using Framework.IoC;
using Hangfire;
using Hangfire.MemoryStorage;
using System;
using System.Threading.Tasks;
using Unity;
using DomainExceptionHandler = FlightAction.ExceptionHandling.DomainExceptionHandler;

namespace FlightAction
{
    static class Program
    {
        private const string RecurringFileUploadJobName = "Flight-Action-Recurring-File-Upload";

        static void Main(string[] args)
        {
            DomainExceptionHandler.HandleDomainExceptions();

            ConfigureUnityContainer();

            GlobalConfigurationSetup();

            ExecuteScheduledJob();

            //CreateScheduler();

            //using var server = new BackgroundJobServer();

            //TickerLoop().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static void GlobalConfigurationSetup()
        {
            GlobalConfiguration.Configuration
                .UseMemoryStorage()
                .UseColouredConsoleLogProvider()
                .UseSerilogLogProvider();

        }

        private static void ConfigureUnityContainer()
        {
            var unityDependencyProvider = new UnityDependencyProvider();
            unityDependencyProvider.RegisterDependencies(new UnityContainer());
        }

        /// <summary>
        /// This will try to download data periodically based on configuration
        /// </summary>
        private static void CreateScheduler()
        {
            string cronExp = "*/5 * * * *";// minute hour day month week
            RecurringJob.AddOrUpdate(RecurringFileUploadJobName, () => ExecuteScheduledJob(), cronExp);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        [DisableConcurrentExecution(1000 * 300)]
        [AutomaticRetry(Attempts = 0)]
        public static void ExecuteScheduledJob()
        {
            var fileUploadService = DependencyUtility.Container.Resolve<IFileUploadService>();
            fileUploadService.ProcessFilesAsync().Wait();
        }

        private static async Task TickerLoop()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }
}
