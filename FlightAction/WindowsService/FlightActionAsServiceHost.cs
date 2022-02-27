using System;
using System.Threading;
using System.Threading.Tasks;
using FlightAction.Core.Services.Interfaces;
using Framework.Utility;
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

        private static readonly AsyncLock AsyncLock = new AsyncLock();

        public FlightActionAsServiceHost(IFileUploadService fileUploadService, IConfiguration configuration, Serilog.ILogger logger)
        {
            _fileUploadService = fileUploadService;
            _configuration = configuration;
            _logger = logger;
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
    }
}
