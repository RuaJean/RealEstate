namespace RealEstate.Application.DTOs.Property
{
    public class PropertyUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public int Year { get; set; }
        public double Area { get; set; }
    }
}
