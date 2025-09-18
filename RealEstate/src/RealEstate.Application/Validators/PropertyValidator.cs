using FluentValidation;
using RealEstate.Application.DTOs.Property;

namespace RealEstate.Application.Validators
{
    public class PropertyCreateValidator : AbstractValidator<PropertyCreateDto>
    {
        public PropertyCreateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.State).MaximumLength(100);
            RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Currency).NotEmpty().Length(3, 4);
            RuleFor(x => x.Year).InclusiveBetween(1800, System.DateTime.UtcNow.Year + 1);
            RuleFor(x => x.Area).GreaterThan(0);
        }
    }

    public class PropertyUpdateValidator : AbstractValidator<PropertyUpdateDto>
    {
        public PropertyUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.State).MaximumLength(100);
            RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Year).InclusiveBetween(1800, System.DateTime.UtcNow.Year + 1);
            RuleFor(x => x.Area).GreaterThan(0);
        }
    }

    public class PropertyPriceUpdateValidator : AbstractValidator<PropertyPriceUpdateDto>
    {
        public PropertyPriceUpdateValidator()
        {
            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Currency).NotEmpty().Length(3, 4);
        }
    }
}


