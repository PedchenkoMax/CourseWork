using Catalog.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Catalog.UnitTests.Domain.Entities;

public class ProductImageEntityTests
{
    [Fact]
    public void TryCreate_ValidParams_ReturnsValidEntity()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var imageUrl = "";
        var displayOrder = 1;

        // Act
        var result = ProductImageEntity.TryCreate(productId, imageUrl, displayOrder, out var entity);

        // Assert
        result.IsValid.Should().BeTrue();
        entity.Should().NotBeNull();
        entity.ProductId.Should().Be(productId);
        entity.ImageUrl.Should().Be(imageUrl);
        entity.DisplayOrder.Should().Be(displayOrder);
    }

    [Fact]
    public void Update_ValidParams_ReturnsValidEntity()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var imageUrl = "";
        var displayOrder = 1;
        
        ProductImageEntity.TryCreate(productId, imageUrl, displayOrder, out var entity);

        // Act
        var newDisplayOrder = 2;

        var result = entity.Update(newDisplayOrder);

        // Assert
        result.IsValid.Should().BeTrue();
        entity.DisplayOrder.Should().Be(newDisplayOrder);
    }
}