using System.Threading.Tasks;

namespace FlightAction.Core.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task ProcessFilesAsync();
    }
}