using System;
using System.Threading;
using System.Threading.Tasks;
using FlightAction.Services.Interfaces;
using Framework.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FlightAction.WindowsService
{
    public class FlightActionAsServiceHost : IHostedService, IDisposable
    {
        private Timer _paymentProcessorTimer;
        private readonly IFileUploadService _fileUploadService;
        private readonly IConfiguration _configuration;
        private readonly Serilog.ILogger _logger;

        public FlightActionAsServiceHost(IFileUploadService fileUploadService, IConfiguration configuration, Serilog.ILogger logger)
        {
            _fileUploadService = fileUploadService;
            _configuration = configuration;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var configuration = DependencyUtility.Resolve<IConfiguration>();

            _logger.Information("Service started");
            var processingIntervalInSeconds = Convert.ToDouble(_configuration["IntervalInSeconds"]);

            _paymentProcessorTimer = new Timer((e) =>
                {
                    _fileUploadService.ProcessFilesAsync();
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
    }
}
