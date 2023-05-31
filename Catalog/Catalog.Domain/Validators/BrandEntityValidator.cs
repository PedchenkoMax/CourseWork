using Catalog.Domain.Entities;
using FluentValidation;

namespace Catalog.Domain.Validators;

public class BrandEntityValidator : AbstractValidator<BrandEntity>
{
    public BrandEntityValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Brand name is required")
            .Length(1, 50).WithMessage("Brand name must be between 1 and 50 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Brand description is required")
            .Length(1, 1000).WithMessage("Brand description must be between 1 and 1000 characters");

        // RuleFor(x => x.ImageFileName)
        // TODO: validate it when image urls are implemented
    }
}