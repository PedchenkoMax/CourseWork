using Catalog.Api.DTO;
using FluentValidation;

namespace Catalog.Api.Validators;

public class ProductParametersValidator : AbstractValidator<ProductParameters>
{
    private readonly IReadOnlyList<string> validOrderByValues = new List<string>
    {
        "Name",
        "Price",
        "Discount",
    };

    public ProductParametersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.OrderBy)
            .NotEmpty().WithMessage("OrderBy is required.")
            .Must(BeValidOrderByValue).WithMessage($"OrderBy must be one of: {string.Join(", ", validOrderByValues)}.");
    }

    private bool BeValidOrderByValue(string orderBy)
    {
        return validOrderByValues.Contains(orderBy);
    }
}