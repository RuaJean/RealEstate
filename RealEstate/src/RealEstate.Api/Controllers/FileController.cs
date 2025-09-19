using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Interfaces;

namespace RealEstate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileStorageService _storage;
        public FileController(IFileStorageService storage)
        {
            _storage = storage;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(104857600)] // 100MB
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Archivo vacío");
            }
            using var stream = file.OpenReadStream();
            var relative = await _storage.SaveFileAsync(stream, file.FileName, file.ContentType, ct);
            var publicPath = _storage.GetPublicUrl(relative);
            var basePath = Request.PathBase.HasValue ? Request.PathBase.Value.TrimEnd('/') : string.Empty;
            var absoluteUrl = $"{Request.Scheme}://{Request.Host}{basePath}{publicPath}";
            return Ok(new { path = relative, url = absoluteUrl });
        }
    }
}
