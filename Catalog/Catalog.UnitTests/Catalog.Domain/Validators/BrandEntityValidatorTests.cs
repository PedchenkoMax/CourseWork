using Catalog.Domain.Validators;
using FluentAssertions;
using Xunit;
using BrandTestHelper = Catalog.UnitTests.TestUtils.SeedData.BrandTestHelper;

namespace Catalog.UnitTests.Catalog.Domain.Validators;

public class BrandEntityValidatorTests
{
    private readonly BrandEntityValidator validator;

    public BrandEntityValidatorTests()
    {
        validator = new BrandEntityValidator();
    }

    [Fact]
    public void Validate_ValidBrandEntity_ShouldNotHaveValidationError()
    {
        // Arrange
        var brandEntity = BrandTestHelper.GetEntity();

        // Act
        var result = validator.Validate(brandEntity);

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
        var brandEntity = BrandTestHelper.GetEntity(name: name);

        // Act
        var result = validator.Validate(brandEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(brandEntity.Name));
    }

    [Fact]
    public void Validate_LongName_ShouldHaveValidationError()
    {
        // Arrange
        var brandEntity = BrandTestHelper.GetEntity(name: new string('A', 51));

        // Act
        var result = validator.Validate(brandEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(brandEntity.Name));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void Validate_EmptyDescription_ShouldHaveValidationError(string description)
    {
        // Arrange
        var brandEntity = BrandTestHelper.GetEntity(description: description);

        // Act
        var result = validator.Validate(brandEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(brandEntity.Description));
    }

    [Fact]
    public void Validate_LongDescription_ShouldHaveValidationError()
    {
        // Arrange
        var brandEntity = BrandTestHelper.GetEntity(description: new string('A', 1001));

        // Act
        var result = validator.Validate(brandEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(brandEntity.Description));
    }
}