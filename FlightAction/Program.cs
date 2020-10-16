using System;
using System.Threading.Tasks;
using FlightAction.IoC;
using FlightAction.Services.Interfaces;
using Framework.IoC;
using Hangfire;
using Hangfire.MemoryStorage;
using Unity;
using DomainExceptionHandler = FlightAction.ExceptionHandling.DomainExceptionHandler;

namespace FlightAction
{
    static class Program
    {
        static void Main(string[] args)
        {
            DomainExceptionHandler.HandleDomainExceptions();

            ConfigureUnityContainer();

            GlobalConfigurationSetup();

            CreateScheduler();

            using var server = new BackgroundJobServer();

            TickerLoop().ConfigureAwait(false).GetAwaiter().GetResult();
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
            RecurringJob.AddOrUpdate(() => ExecuteScheduledJob(), cronExp);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static void ExecuteScheduledJob()
        {
            var fileUploadService = DependencyUtility.Container.Resolve<IFileUploadService>();
            var result = fileUploadService.GetShows().Result;
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
