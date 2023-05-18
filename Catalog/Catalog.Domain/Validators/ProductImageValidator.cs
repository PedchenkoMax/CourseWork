using Catalog.Domain.Entities;
using FluentValidation;

namespace Catalog.Domain.Validators;

public class ProductImageEntityValidator : AbstractValidator<ProductImageEntity>
{
    public ProductImageEntityValidator()
    {
        // RuleFor(x => x.ImageUrl)
        // TODO: validate it when image urls are implemented
        
        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be a non-negative integer");
    }
}