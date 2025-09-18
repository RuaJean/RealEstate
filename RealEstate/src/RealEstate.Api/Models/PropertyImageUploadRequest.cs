using Microsoft.AspNetCore.Http;

namespace RealEstate.Api.Models
{
    public sealed class PropertyImageUploadRequest
    {
        public IFormFile File { get; set; } = default!;
        public string? Description { get; set; }
        public bool Enabled { get; set; } = true;
    }
}


