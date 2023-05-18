using Catalog.Domain.Entities;
using FluentValidation;

namespace Catalog.Domain.Validators;

public class ProductEntityValidator : AbstractValidator<ProductEntity>
{
    public ProductEntityValidator()
    {
        RuleFor(product => product.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(1, 50).WithMessage("Name must be between 1 and 50 characters");

        RuleFor(product => product.Description)
            .NotEmpty().WithMessage("Description is required")
            .Length(1, 1000).WithMessage("Description must be between 1 and 1000 characters");

        RuleFor(product => product.Price)
            .GreaterThan(0).WithMessage("Price must be a non-negative number");

        RuleFor(product => product.Discount)
            .InclusiveBetween(0, 1).WithMessage("Discount must be between 0 and 1");

        RuleFor(product => product.SKU)
            .Length(10).WithMessage("SKU must be exactly 10 characters long");

        RuleFor(product => product.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock must be a non-negative integer");

        RuleFor(product => product.Availability)
            .NotNull().WithMessage("Availability must not be null");
    }
}