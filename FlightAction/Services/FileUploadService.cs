using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FlightAction.Api;
using FlightAction.DTO;
using FlightAction.Services.Interfaces;
using Flurl.Http;
using Flurl.Http.Content;
using Framework.Extensions;
using Framework.Utility;
using Framework.Utility.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace FlightAction.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IDirectoryUtility _directoryUtility;
        private readonly ILogger _logger;
        private readonly Lazy<string> _baseUrl;
        private readonly Lazy<string> _apiKey;
        private readonly Lazy<string> _parentFileLocation;
        private readonly Lazy<string> _processedFileLocation;

        private const string NoNewFileToUploadMessage = "No new files available to upload";

        public FileUploadService(Lazy<IConfiguration> configuration, IDirectoryUtility directoryUtility, ILogger logger)
        {
            _baseUrl = new Lazy<string>(configuration.Value["ServerHost"]);
            _parentFileLocation = new Lazy<string>(configuration.Value["ParentFileLocation"]);
            _processedFileLocation = new Lazy<string>(configuration.Value["ProcessedFileLocation"]);

            _directoryUtility = directoryUtility;
            _logger = logger;
        }

        public async Task ProcessFilesAsync()
        {
            await TryCatchExtension.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    var getResult = _directoryUtility.GetAllFilesInDirectory(_parentFileLocation.Value);
                    if (getResult.HasNoValue)
                    {
                        _logger.Information(NoNewFileToUploadMessage);
                    }

                    var currentProcessedDirectory = PrepareProcessedDirectory();

                    foreach (var item in getResult.Value)
                    {
                        var fileUploadResult = await UploadFileToServerAsync(item);
                        if (fileUploadResult.IsSuccess)
                            _directoryUtility.Move(item, Path.Combine(currentProcessedDirectory, Path.GetFileName(item)));
                    }
                },
                ex =>
                {
                    _logger.Fatal(ex, $"Error occured in {nameof(ProcessFilesAsync)}. Exception Message:{ex.Message}. Details: {ex.GetExceptionDetailMessage()}");
                    return false;
                });
        }

        private string PrepareProcessedDirectory()
        {
            var currentProcessedDirectory = Path.Combine(_processedFileLocation.Value, DateTime.Now.ToString(Constants.DateFormatter.yyyy_MM_dd_Dash_Delimited));

            _directoryUtility.CreateFolderIfNotExistAsync(_processedFileLocation.Value);
            _directoryUtility.CreateFolderIfNotExistAsync(currentProcessedDirectory);

            return currentProcessedDirectory;
        }

        private async Task<Result<bool>> UploadFileToServerAsync(string fileName)
        {
            var result = false;

            await TryCatchExtension.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    var json = FlurlHttp.GlobalSettings.JsonSerializer.Serialize(new FileUploadDTO
                    {
                        FileName = fileName,
                        FileBytes = File.ReadAllBytes(fileName)
                    });

                    var content = new CapturedStringContent(json, Encoding.UTF8, "application/json-patch+json");

                    result = await _baseUrl
                             .Value
                             .WithHeader(ApiCollection.DefaultHeader, ApiCollection.FileUploadApi.DefaultVersion)
                             .AppendPathSegment(ApiCollection.FileUploadApi.Segment)
                             .PostAsync(content).ReceiveJson<bool>();
                },
                ex =>
                {
                    _logger.Fatal(ex, $"Error occured in {nameof(UploadFileToServerAsync)}. Exception Message:{ex.Message}. Details: {ex.GetExceptionDetailMessage()}");
                    return false;
                });

            return result ? Result.Success(true) : Result.Failure<bool>("File upload failed. Please check log");
        }
    }
}
