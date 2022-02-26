using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using FlightAction.Core;
using FlightAction.ExceptionHandling;
using FlightAction.Services.Interfaces;
using Framework.Utility;
using Microsoft.Extensions.Configuration;
using Serilog;
using Unity;

namespace AutoCount
{
    public partial class Service1 : ServiceBase
    {
        private Timer _paymentProcessorTimer;
        private IFileUploadService _fileUploadService;
        private IConfiguration _configuration;
        private ILogger _logger;

        private static readonly AsyncLock AsyncLock = new AsyncLock();

        public Service1()
        {
            InitializeComponent();

            //DomainExceptionHandler.HandleDomainExceptions();

            //InitialSetup.GlobalConfigurationSetup();

            //var unityContainer = InitialSetup.ConfigureUnityContainer();

            ////INFO: Don't move this method from here.
            //InitialSetup.ConfigureFlurlHttpClient(unityContainer);


            //_fileUploadService = unityContainer.Resolve<IFileUploadService>();
            //_configuration = unityContainer.Resolve<IConfiguration>();
            //_logger = unityContainer.Resolve<ILogger>();

            //var _baseUrl = _configuration["ServerHost"];
            //var _userName = _configuration["UserId"];
            //var _password = _configuration["Password"];
            //var _fileLocation = new FileLocation
            //{
            //    Air = _configuration.GetSection("FileLocation:Air").Value,
            //    Pnr = _configuration.GetSection("FileLocation:Pnr").Value,
            //    Mir = _configuration.GetSection("FileLocation:Mir").Value
            //};

            //_logger.Information("Service started");
        }

        protected override void OnStart(string[] args)
        {
            DebugMode();
            /* ... do the rest */

            DomainExceptionHandler.HandleDomainExceptions();

            InitialSetup.GlobalConfigurationSetup();

            var unityContainer = InitialSetup.ConfigureUnityContainer();

            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule?.FileName;

                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                if (pathToContentRoot != null)
                    Directory.SetCurrentDirectory(pathToContentRoot);
            }

            _fileUploadService = unityContainer.Resolve<IFileUploadService>();
            _configuration = unityContainer.Resolve<IConfiguration>();
            _logger = unityContainer.Resolve<ILogger>();

            //INFO: Don't move this method from here.
            InitialSetup.ConfigureFlurlHttpClient(unityContainer);
        }

        protected override void OnStop()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Service started");

            var processingIntervalInSeconds = Convert.ToDouble(_configuration["IntervalInSeconds"]);

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

        public void Dispose()
        {
            _paymentProcessorTimer?.Dispose();
        }

        [Conditional("DEBUG_SERVICE")]
        private static void DebugMode()
        {
            Debugger.Break();
        }
    }
}
