using Catalog.Domain.Validators;
using FluentAssertions;
using Xunit;
using CategoryTestHelper = Catalog.UnitTests.TestUtils.SeedData.CategoryTestHelper;

namespace Catalog.UnitTests.Catalog.Domain.Validators;

public class CategoryEntityValidatorTests
{
    private readonly CategoryEntityValidator validator;

    public CategoryEntityValidatorTests()
    {
        validator = new CategoryEntityValidator();
    }

    [Fact]
    public void Validate_ValidCategoryEntity_ShouldNotHaveValidationError()
    {
        // Arrange
        var categoryEntity = CategoryTestHelper.GetEntity();

        // Act
        var result = validator.Validate(categoryEntity);

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
        var categoryEntity = CategoryTestHelper.GetEntity(name: name);

        // Act
        var result = validator.Validate(categoryEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(categoryEntity.Name));
    }

    [Fact]
    public void Validate_LongName_ShouldHaveValidationError()
    {
        // Arrange
        var categoryEntity = CategoryTestHelper.GetEntity(name: new string('A', 51));

        // Act
        var result = validator.Validate(categoryEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(categoryEntity.Name));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void Validate_EmptyDescription_ShouldHaveValidationError(string description)
    {
        // Arrange
        var categoryEntity = CategoryTestHelper.GetEntity(description: description);

        // Act
        var result = validator.Validate(categoryEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(categoryEntity.Description));
    }

    [Fact]
    public void Validate_LongDescription_ShouldHaveValidationError()
    {
        // Arrange
        var categoryEntity = CategoryTestHelper.GetEntity(description: new string('A', 1001));

        // Act
        var result = validator.Validate(categoryEntity);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(categoryEntity.Description));
    }
}