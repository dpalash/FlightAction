using System.Collections.Generic;
using System.Threading.Tasks;
using FlightAction.Services.Interfaces;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;

namespace FlightAction.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly string _baseUrl;
        private readonly string _apiKey;

        public FileUploadService(IConfiguration configuration)
        {
            _baseUrl = configuration["ServerHost"];
        }

        public Task<List<string>> GetShows()
        {
            return _baseUrl
                .AppendPathSegment("v1/podcasts/shownum/episodes.json")
                .SetQueryParam("api_key", _apiKey)
                .GetJsonAsync<List<string>>();
        }
    }

}
