using Catalog.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Catalog.UnitTests.Catalog.Domain.Entities;

public class CategoryEntityTests
{
    [Fact]
    public void TryCreate_ValidParams_ReturnsValidEntity()
    {
        // Arrange
        Guid? parentCategoryId = null;
        var name = "Test Category";
        var description = "Test Description";
        var imageUrl = "";

        // Act
        var result = CategoryEntity.TryCreate(parentCategoryId, name, description, imageUrl, out var entity);

        // Assert
        result.IsValid.Should().BeTrue();
        entity.Should().NotBeNull();
        entity.ParentCategoryId.Should().Be(parentCategoryId);
        entity.Name.Should().Be(name);
        entity.Description.Should().Be(description);
        entity.ImageFileName.Should().Be(imageUrl);
    }

    [Fact]
    public void Update_ValidParams_ReturnsValidEntity()
    {
        // Arrange
        Guid? parentCategoryId = null;
        var name = "Test Category";
        var description = "Test Description";
        var imageUrl = "";

        CategoryEntity.TryCreate(parentCategoryId, name, description, imageUrl, out var entity);

        // Act
        Guid? newParentCategoryId = null;
        var newName = "Updated Category";
        var newDescription = "Updated Description";
        var newImageUrl = "";

        var result = entity.Update(newParentCategoryId, newName, newDescription, newImageUrl);

        // Assert
        result.IsValid.Should().BeTrue();
        entity.ParentCategoryId.Should().Be(newParentCategoryId);
        entity.Name.Should().Be(newName);
        entity.Description.Should().Be(newDescription);
        entity.ImageFileName.Should().Be(newImageUrl);
    }
}