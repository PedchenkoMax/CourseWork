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

        // Act
        var result = BrandEntity.TryCreate(name, description, imageUrl, out var entity);

        // Assert
        result.IsValid.Should().BeTrue();
        entity.Should().NotBeNull();
        entity.Name.Should().Be(name);
        entity.Description.Should().Be(description);
        entity.ImageFileName.Should().Be(imageUrl);
    }

    [Fact]
    public void Update_ValidParams_ReturnsValidEntity()
    {
        // Arrange
        var name = "Test Brand";
        var description = "Test Description";
        var imageUrl = "";

        BrandEntity.TryCreate(name, description, imageUrl, out var entity);

        // Act
        var newName = "Updated Brand";
        var newDescription = "Updated Description";
        var newImageUrl = "";

        var result = entity.Update(newName, newDescription, newImageUrl);

        // Assert
        result.IsValid.Should().BeTrue();
        entity.Name.Should().Be(newName);
        entity.Description.Should().Be(newDescription);
        entity.ImageFileName.Should().Be(newImageUrl);
    }
}