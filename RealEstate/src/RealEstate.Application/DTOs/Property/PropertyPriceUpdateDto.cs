namespace RealEstate.Application.DTOs.Property
{
    public class PropertyPriceUpdateDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
    }
}
