using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using RealEstate.Application.Interfaces;

namespace RealEstate.Infrastructure.Storage
{
    public sealed class LocalFileStorageOptions
    {
        public string RootPath { get; set; } = "wwwroot"; // relativa al content root por defecto
        public string BaseRequestPath { get; set; } = "/"; // prefijo público
    }

    public sealed class LocalFileStorage : IFileStorageService
    {
        private readonly LocalFileStorageOptions _options;
        private readonly string _contentRootPath;

        public LocalFileStorage(LocalFileStorageOptions options, string contentRootPath)
        {
            _options = options;
            _contentRootPath = contentRootPath;
        }

        public async Task<string> SaveFileAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            string safeExt = Path.GetExtension(fileName);
            string subfolder = Path.Combine("uploads", DateTime.UtcNow.ToString("yyyy", CultureInfo.InvariantCulture), DateTime.UtcNow.ToString("MM", CultureInfo.InvariantCulture), DateTime.UtcNow.ToString("dd", CultureInfo.InvariantCulture));
            string relativePath = Path.Combine(subfolder, $"{Guid.NewGuid():N}{safeExt}").Replace('\\', '/');

            string root = GetAbsoluteRootPath();
            string fullDir = Path.Combine(root, subfolder);
            Directory.CreateDirectory(fullDir);

            string fullPath = Path.Combine(root, relativePath);
            using (var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                await content.CopyToAsync(fs, cancellationToken);
            }

            return relativePath.Replace('\\', '/');
        }

        public Task<bool> DeleteFileAsync(string relativePath, CancellationToken cancellationToken = default)
        {
            string fullPath = Path.Combine(GetAbsoluteRootPath(), relativePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public string GetPublicUrl(string relativePath)
        {
            string basePath = _options.BaseRequestPath?.TrimEnd('/') ?? string.Empty;
            string rel = relativePath.TrimStart('/');
            return $"{basePath}/{rel}";
        }

        private string GetAbsoluteRootPath()
        {
            string root = _options.RootPath;
            if (Path.IsPathRooted(root))
            {
                return root;
            }
            return Path.GetFullPath(Path.Combine(_contentRootPath, root));
        }
    }
}


