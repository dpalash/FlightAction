using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using FlightAction.Core;
using FlightAction.Core.ExceptionHandling;
using FlightAction.Core.Services.Interfaces;
using Framework.Utility;
using Microsoft.Extensions.Configuration;
using Serilog;
using Unity;

namespace AutoCount
{
    public partial class AutoCountService : ServiceBase
    {
        private Timer _paymentProcessorTimer;
        private IFileUploadService _fileUploadService;
        private IConfiguration _configuration;
        private ILogger _logger;

        private static readonly AsyncLock AsyncLock = new AsyncLock();

        public AutoCountService()
        {
            InitializeComponent();

            PrepareInitialSetups();
        }

        private void PrepareInitialSetups()
        {
            DomainExceptionHandler.HandleDomainExceptions();

            InitialSetup.GlobalConfigurationSetup();

            var unityContainer = InitialSetup.ConfigureUnityContainer();

            //INFO: Don't move this method from here.
            InitialSetup.ConfigureFlurlHttpClient(unityContainer);

            _fileUploadService = unityContainer.Resolve<IFileUploadService>();
            _configuration = unityContainer.Resolve<IConfiguration>();
            _logger = unityContainer.Resolve<ILogger>();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
#if DEBUG
            Debugger.Launch(); // launch and attach debugger
#endif

            StartAsync(new CancellationToken()).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        protected override void OnStop()
        {
            StopAsync(new CancellationToken()).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service started");

            //var processingIntervalInSeconds = Convert.ToDouble(_configuration["IntervalInSeconds"]);
            var processingIntervalInSeconds = Convert.ToDouble(System.Configuration.ConfigurationSettings.AppSettings["IntervalInSeconds"]);

            _paymentProcessorTimer = new Timer(async e =>
                {
                    // This means this lock instance has already occupied the allocation 1 thread. No available lock instance is available.
                    if (AsyncLock.CurrentCount() == 0)
                        return;

                    using (await AsyncLock.LockAsync())
                    {
                        await _fileUploadService.ProcessFilesAsync();
                    }
                }, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(processingIntervalInSeconds));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service stopped");
            _paymentProcessorTimer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
