using System;

namespace RealEstate.Application.DTOs.Property
{
    public class PropertyFilterDto
    {
        public Guid? OwnerId { get; set; }
        public string? Text { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public int? Year { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
