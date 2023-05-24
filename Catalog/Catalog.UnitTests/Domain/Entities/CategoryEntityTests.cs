using Catalog.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Catalog.UnitTests.Domain.Entities;

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
        var displayOrder = 1;

        // Act
        var result = CategoryEntity.TryCreate(parentCategoryId, name, description, imageUrl, displayOrder, out var entity);

        // Assert
        result.IsValid.Should().BeTrue();
        entity.Should().NotBeNull();
        entity.ParentCategoryId.Should().Be(parentCategoryId);
        entity.Name.Should().Be(name);
        entity.Description.Should().Be(description);
        entity.ImageUrl.Should().Be(imageUrl);
        entity.DisplayOrder.Should().Be(displayOrder);
    }

    [Fact]
    public void Update_ValidParams_ReturnsValidEntity()
    {
        // Arrange
        Guid? parentCategoryId = null;
        var name = "Test Category";
        var description = "Test Description";
        var imageUrl = "";
        var displayOrder = 1;
        
        CategoryEntity.TryCreate(parentCategoryId, name, description, imageUrl, displayOrder, out var entity);

        // Act
        Guid? newParentCategoryId = null;
        var newName = "Updated Category";
        var newDescription = "Updated Description";
        var newImageUrl = "";
        var newDisplayOrder = 2;

        var result = entity.Update(newParentCategoryId, newName, newDescription, newImageUrl, newDisplayOrder);

        // Assert
        result.IsValid.Should().BeTrue();
        entity.ParentCategoryId.Should().Be(newParentCategoryId);
        entity.Name.Should().Be(newName);
        entity.Description.Should().Be(newDescription);
        entity.ImageUrl.Should().Be(newImageUrl);
        entity.DisplayOrder.Should().Be(newDisplayOrder);
    }
}