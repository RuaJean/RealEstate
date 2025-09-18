using FluentValidation;
using RealEstate.Application.DTOs.PropertyImage;

namespace RealEstate.Application.Validators
{
    public class PropertyImageCreateValidator : AbstractValidator<PropertyImageCreateDto>
    {
        public PropertyImageCreateValidator()
        {
            RuleFor(x => x.PropertyId).NotEmpty();
            RuleFor(x => x.Url).NotEmpty().MaximumLength(1000).Must(u => Uri.TryCreate(u, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                .WithMessage("URL inválida");
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }
}


