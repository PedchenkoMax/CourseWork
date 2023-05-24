using Catalog.Domain.Validators;
using Catalog.IntegrationTests.SeedData;
using FluentAssertions;
using Xunit;
using ProductImageTestHelper = Catalog.UnitTests.SeedData.ProductImageTestHelper;

namespace Catalog.UnitTests.Domain.Validators;

public class ProductImageEntityValidatorTests
{
    private readonly ProductImageEntityValidator validator;

    public ProductImageEntityValidatorTests()
    {
        validator = new ProductImageEntityValidator();
    }

    [Fact]
    public void Validate_ValidProductImageEntity_ShouldNotHaveValidationError()
    {
        // Arrange
        var productImageEntity = ProductImageTestHelper.GetEntity();

        // Act
        var result = validator.Validate(productImageEntity);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NegativeDisplayOrder_ShouldHaveValidationError()
    {
        // Arrange
        var productImageEntity = ProductImageTestHelper.GetEntity(displayOrder: -1);

        // Act
        var result = validator.Validate(productImageEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(productImageEntity.DisplayOrder));
    }
}