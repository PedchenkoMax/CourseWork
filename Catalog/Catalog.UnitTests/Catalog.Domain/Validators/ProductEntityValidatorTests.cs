using Catalog.Domain.Validators;
using FluentAssertions;
using Xunit;
using ProductTestHelper = Catalog.UnitTests.TestUtils.SeedData.ProductTestHelper;

namespace Catalog.UnitTests.Catalog.Domain.Validators;

public class ProductEntityValidatorTests
{
    private readonly ProductEntityValidator validator;

    public ProductEntityValidatorTests()
    {
        validator = new ProductEntityValidator();
    }

    [Fact]
    public void Validate_ValidProductEntity_ShouldNotHaveValidationError()
    {
        // Arrange
        var productEntity = ProductTestHelper.GetEntity();

        // Act
        var result = validator.Validate(productEntity);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void Validate_EmptyName_ShouldHaveValidationError(string name)
    {
        // Arrange
        var productEntity = ProductTestHelper.GetEntity(name: name);

        // Act
        var result = validator.Validate(productEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(productEntity.Name));
    }

    [Fact]
    public void Validate_LongName_ShouldHaveValidationError()
    {
        // Arrange
        var validEntity = ProductTestHelper.GetEntity(name: new string('A', 51));

        // Act
        var result = validator.Validate(validEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(validEntity.Name));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void Validate_EmptyDescription_ShouldHaveValidationError(string description)
    {
        // Arrange
        var productEntity = ProductTestHelper.GetEntity(description: description);

        // Act
        var result = validator.Validate(productEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(productEntity.Description));
    }

    [Fact]
    public void Validate_LongDescription_ShouldHaveValidationError()
    {
        // Arrange
        var validEntity = ProductTestHelper.GetEntity(description: new string('A', 1001));

        // Act
        var result = validator.Validate(validEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(validEntity.Description));
    }

    [Fact]
    public void Validate_NegativePrice_ShouldHaveValidationError()
    {
        // Arrange
        var productEntity = ProductTestHelper.GetEntity(price: -10);

        // Act
        var result = validator.Validate(productEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(productEntity.Price));
    }

    [Theory]
    [InlineData(-1.0)]
    [InlineData(1.1)]
    public void Validate_InvalidDiscount_ShouldHaveValidationError(decimal discount)
    {
        // Arrange
        var productEntity = ProductTestHelper.GetEntity(discount: discount);

        // Act
        var result = validator.Validate(productEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(productEntity.Discount));
    }

    [Fact]
    public void Validate_InvalidSKU_ShouldHaveValidationError()
    {
        // Arrange
        var productEntity = ProductTestHelper.GetEntity(sku: "123456789");

        // Act
        var result = validator.Validate(productEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(productEntity.SKU));
    }

    [Fact]
    public void Validate_NegativeStock_ShouldHaveValidationError()
    {
        // Arrange
        var productEntity = ProductTestHelper.GetEntity(stock: -1);

        // Act
        var result = validator.Validate(productEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(productEntity.Stock));
    }
}