using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FlightAction.Api;
using FlightAction.DTO;
using FlightAction.DTO.Enum;
using FlightAction.Services.Interfaces;
using Flurl.Http;
using Flurl.Http.Content;
using Framework.Base.ModelEntity;
using Framework.Extensions;
using Framework.Models;
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
        private readonly Lazy<FileLocation> _fileLocation;

        private const string ProcessedFileLocation = "Processed";
        private const string NoNewFileToUploadMessage = "No new files available to upload";

        public FileUploadService(Lazy<IConfiguration> configuration, IDirectoryUtility directoryUtility, ILogger logger)
        {
            _baseUrl = new Lazy<string>(configuration.Value["ServerHost"]);
            _fileLocation = new Lazy<FileLocation>(configuration.Value.GetSection("FileLocation").Get<FileLocation>());

            _directoryUtility = directoryUtility;
            _logger = logger;
        }

        public async Task ProcessFilesAsync()
        {
            await TryCatchExtension.ExecuteAndHandleErrorAsync(
                async () =>
                {
                    foreach (var prop in _fileLocation.Value.GetType().GetProperties())
                    {
                        var currentDirectory = prop.GetValue(_fileLocation.Value, null).ToString();
                        var getResult = _directoryUtility.GetAllFilesInDirectory(currentDirectory);
                        if (getResult.HasNoValue)
                        {
                            _logger.Information(NoNewFileToUploadMessage);
                            continue;
                        }

                        var currentProcessedDirectory = PrepareProcessedDirectory(currentDirectory);

                        foreach (var filePath in getResult.Value)
                        {
                            var fileUploadResult = await UploadFileToServerAsync(filePath);
                            if (fileUploadResult.IsSuccess)
                                _directoryUtility.Move(filePath, Path.Combine(currentProcessedDirectory, Path.GetFileName(filePath)));
                        }
                    }
                },
                ex =>
                {
                    _logger.Fatal(ex, $"Error occured in {nameof(ProcessFilesAsync)}. Exception Message:{ex.Message}. Details: {ex.GetExceptionDetailMessage()}");
                    return false;
                });
        }

        private string PrepareProcessedDirectory(string currentDirectory)
        {
            var currentProcessedDirectory = Path.Combine(currentDirectory, ProcessedFileLocation, DateTime.Now.ToString(Constants.DateFormatter.yyyy_MM_dd_Dash_Delimited));

            _directoryUtility.CreateFolderIfNotExistAsync(ProcessedFileLocation);
            _directoryUtility.CreateFolderIfNotExistAsync(currentProcessedDirectory);

            return currentProcessedDirectory;
        }

        private async Task<Result<bool>> UploadFileToServerAsync(string filePath)
        {
            return await TryCatchExtension.ExecuteAndHandleErrorAsync(
                 async () =>
                 {
                     var json = FlurlHttp.GlobalSettings.JsonSerializer.Serialize(new AuthenticateRequestDTO
                     {
                         UserName = "DemoE01",
                         Password = "12345"
                     });

                     var content = new CapturedStringContent(json, Encoding.UTF8, "application/json-patch+json");

                     var authenticateResponse = await _baseUrl
                         .Value
                         .WithHeader(ApiCollection.DefaultHeader, ApiCollection.FileUploadApi.DefaultVersion)
                         .AppendPathSegment(ApiCollection.AuthenticationApi.Segment)
                         .PostAsync(content).ReceiveJson<PrometheusResponse>();

                    var responseData = authenticateResponse.Data.ToString().DeserializeObject<AuthenticateResponseDTO>();

                     json = FlurlHttp.GlobalSettings.JsonSerializer.Serialize(new TicketFileDTO
                     {
                         FileName = Path.GetFileName(filePath),
                         FileType = GetFileType(filePath),
                         MachineInfoDTO = MachineInfoDTO.Create(),
                         EmployeeId = responseData.EmployeeId,
                         CompanyId = responseData.CompanyId,
                         FileBytes = File.ReadAllBytes(filePath)
                     });

                     content = new CapturedStringContent(json, Encoding.UTF8, "application/json-patch+json");

                     var result = await _baseUrl
                              .Value
                              .WithHeader(ApiCollection.DefaultHeader, ApiCollection.FileUploadApi.DefaultVersion)
                              .WithHeader("Authorization", $"Bearer {responseData.Token}")
                              .AppendPathSegment(ApiCollection.FileUploadApi.Segment)
                              .PostAsync(content).ReceiveJson<PrometheusResponse>();

                     return result.StatusCode == HttpStatusCode.OK ? Result.Success(true) : Result.Failure<bool>("File upload failed. Please check log");
                 },
                 ex => new TryCatchExtensionResult<Result<bool>>
                 {
                     AdditionalAction = () =>
                     {
                         _logger.Fatal(ex, $"Error occured in {nameof(UploadFileToServerAsync)}. Exception Message:{ex.Message}. Details: {ex.GetExceptionDetailMessage()}");
                     },

                     DefaultResult = Result.Failure<bool>($"Error message: {ex.Message}. Details: {ex.GetExceptionDetailMessage()}"),
                     RethrowException = false
                 });
        }

        private FileTypeEnum GetFileType(string filePath)
        {
            var fileType = FileTypeEnum.Air;

            switch (filePath)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                case var _ when filePath.ToLower().Contains(Enum.GetName(typeof(FileTypeEnum), FileTypeEnum.Air)?.ToLower()):
                    fileType = FileTypeEnum.Air;
                    break;

                // ReSharper disable once AssignNullToNotNullAttribute
                case var _ when filePath.ToLower().Contains(Enum.GetName(typeof(FileTypeEnum), FileTypeEnum.Mir)?.ToLower()):
                    fileType = FileTypeEnum.Mir;
                    break;

                // ReSharper disable once AssignNullToNotNullAttribute
                case var _ when filePath.ToLower().Contains(Enum.GetName(typeof(FileTypeEnum), FileTypeEnum.Pnr)?.ToLower()):
                    fileType = FileTypeEnum.Pnr;
                    break;

            }

            return fileType;
        }
    }
}
