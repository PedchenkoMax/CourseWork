using System.ComponentModel.DataAnnotations;

namespace Catalog.Api.ValidationAttributes;

public class NonZeroGuidAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext? validationContext)
    {
        if (value is not Guid guidValue || guidValue == Guid.Empty)
        {
            return validationContext?.MemberName == null
                ? new ValidationResult(ErrorMessage, null)
                : new ValidationResult(ErrorMessage, new[] { validationContext.MemberName });
        }

        return ValidationResult.Success!;
    }
}