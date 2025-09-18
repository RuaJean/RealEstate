using System;

namespace RealEstate.Application.DTOs.Property
{
    public class PropertyCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";
        public int Year { get; set; }
        public double Area { get; set; }
        public Guid OwnerId { get; set; }
        public bool Active { get; set; } = true;
    }
}


