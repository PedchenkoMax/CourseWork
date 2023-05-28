using Catalog.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Catalog.UnitTests.Catalog.Domain.Entities;

public class BrandEntityTests
{
    [Fact]
    public void TryCreate_ValidParams_ReturnsValidEntity()
    {
        // Arrange
        var name = "Test Brand";
        var description = "Test Description";
        var imageUrl = "";
        var displayOrder = 1;

        // Act
        var result = BrandEntity.TryCreate(name, description, imageUrl, displayOrder, out var entity);

        // Assert
        result.IsValid.Should().BeTrue();
        entity.Should().NotBeNull();
        entity.Name.Should().Be(name);
        entity.Description.Should().Be(description);
        entity.ImageFileName.Should().Be(imageUrl);
        entity.DisplayOrder.Should().Be(displayOrder);
    }

    [Fact]
    public void Update_ValidParams_ReturnsValidEntity()
    {
        // Arrange
        var name = "Test Brand";
        var description = "Test Description";
        var imageUrl = "";
        var displayOrder = 1;

        BrandEntity.TryCreate(name, description, imageUrl, displayOrder, out var entity);

        // Act
        var newName = "Updated Brand";
        var newDescription = "Updated Description";
        var newImageUrl = "";
        var newDisplayOrder = 2;

        var result = entity.Update(newName, newDescription, newImageUrl, newDisplayOrder);

        // Assert
        result.IsValid.Should().BeTrue();
        entity.Name.Should().Be(newName);
        entity.Description.Should().Be(newDescription);
        entity.ImageFileName.Should().Be(newImageUrl);
        entity.DisplayOrder.Should().Be(newDisplayOrder);
    }
}