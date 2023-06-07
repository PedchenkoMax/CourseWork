using FluentValidation;

namespace Catalog.Api.Validators;

public class ProductImageFileValidator : AbstractValidator<IFormFile>
{
    public ProductImageFileValidator()
    {
        // TODO: add file validation rules
    }
}