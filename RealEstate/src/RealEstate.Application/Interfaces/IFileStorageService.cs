using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RealEstate.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default);
        Task<bool> DeleteFileAsync(string relativePath, CancellationToken cancellationToken = default);
        string GetPublicUrl(string relativePath);
    }
}
