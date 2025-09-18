using System;

namespace RealEstate.Application.DTOs.PropertyTrace
{
    public class PropertyTraceCreateDto
    {
        public Guid PropertyId { get; set; }
        public DateTime DateUtc { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Currency { get; set; } = "USD";
    }
}


