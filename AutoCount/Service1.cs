using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlightAction.Core;
using FlightAction.ExceptionHandling;
using Framework.Utility;

namespace AutoCount
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();

            DomainExceptionHandler.HandleDomainExceptions();

            InitialSetup.GlobalConfigurationSetup();

            var unityContainer = InitialSetup.ConfigureUnityContainer();

            //INFO: Don't move this method from here.
            InitialSetup.ConfigureFlurlHttpClient(unityContainer);
        }

        protected override void OnStart(string[] args)
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

            //INFO: Don't move this method from here.
            InitialSetup.ConfigureFlurlHttpClient(unityContainer);
        }

        protected override void OnStop()
        {
        }

        //public Task StartAsync(CancellationToken cancellationToken)
        //{
        //    _logger.Information("Service started");
        //    var processingIntervalInSeconds = Convert.ToDouble(_configuration["IntervalInSeconds"]);

        //    _paymentProcessorTimer = new Timer(async e =>
        //        {
        //            // This means this lock instance has already occupied the allocation 1 thread. No available lock instance is available.
        //            if (AsyncLock.CurrentCount() == 0)
        //                return;

        //            using (await AsyncLock.LockAsync())
        //            {
        //                await _fileUploadService.ProcessFilesAsync();
        //            }
        //        }, null, TimeSpan.Zero,
        //        TimeSpan.FromSeconds(processingIntervalInSeconds));

        //    return Task.CompletedTask;
        //}

        //public Task StopAsync(CancellationToken cancellationToken)
        //{
        //    _logger.Information("Service stopped");
        //    _paymentProcessorTimer?.Change(Timeout.Infinite, 0);
        //    return Task.CompletedTask;
        //}

        //public void Dispose()
        //{
        //    _paymentProcessorTimer?.Dispose();
        //}
    }
}
