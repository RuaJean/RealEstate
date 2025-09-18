using System;

namespace RealEstate.Application.DTOs.Owner
{
    public class OwnerResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}
