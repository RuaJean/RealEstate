using FluentValidation;
using RealEstate.Application.DTOs.Owner;

namespace RealEstate.Application.Validators
{
    public class OwnerCreateValidator : AbstractValidator<OwnerCreateDto>
    {
        public OwnerCreateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
            RuleFor(x => x.Photo).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Photo));
        }
    }

    public class OwnerUpdateValidator : AbstractValidator<OwnerUpdateDto>
    {
        public OwnerUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
            RuleFor(x => x.Photo).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Photo));
        }
    }
}
