using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightAction.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task<List<string>> GetShows();
    }
}