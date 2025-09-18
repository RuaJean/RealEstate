using System;

namespace RealEstate.Application.DTOs.PropertyImage
{
    public class PropertyImageResponseDto
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}


