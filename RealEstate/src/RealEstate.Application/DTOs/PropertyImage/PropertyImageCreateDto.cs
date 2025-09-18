using System;

namespace RealEstate.Application.DTOs.PropertyImage
{
    public class PropertyImageCreateDto
    {
        public Guid PropertyId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
    }
}


