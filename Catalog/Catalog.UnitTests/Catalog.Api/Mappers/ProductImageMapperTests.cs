using Catalog.Api.Mappers;
using Catalog.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Catalog.UnitTests.Catalog.Api.Mappers;

public class ProductImageMapperTests
{
    // [Fact]
    // public void MapToReadDto_ValidEntity_ReturnsValidDto()
    // {
    //     // Arrange
    //     ProductImageEntity.TryCreate
    //     (
    //         productId: Guid.NewGuid(),
    //         imageFileName: "",
    //         displayOrder: 1,
    //         out var entity
    //     );
    //
    //     // Act
    //     var dto = ProductImageMapper.MapToReadDto(entity);
    //
    //     // Assert
    //     dto.Should().NotBeNull();
    //     dto.Id.Should().Be(entity.Id);
    //     dto.ProductId.Should().Be(entity.ProductId);
    //     dto.ImageUrl.Should().Be(entity.ImageFileName);
    //     dto.DisplayOrder.Should().Be(entity.DisplayOrder);
    // }
    //
    // [Fact]
    // public void TryCreateEntity_ValidDto_ReturnsValidEntity()
    // {
    //     // Arrange
    //     var dto = new ProductImageWriteDto
    //     (
    //         ProductId: Guid.NewGuid(),
    //         ImageFileName: "",
    //         DisplayOrder: 1
    //     );
    //
    //     // Act
    //     var result = ProductImageMapper.TryCreateEntity(dto, out var entity);
    //
    //     // Assert
    //     result.IsValid.Should().BeTrue();
    //     entity.Should().NotBeNull();
    //     entity.ProductId.Should().Be(dto.ProductId);
    //     entity.ImageFileName.Should().Be(dto.ImageFileName);
    //     entity.DisplayOrder.Should().Be(dto.DisplayOrder);
    // }
    //
    // [Fact]
    // public void TryUpdateEntity_ValidDto_ReturnsValidEntity()
    // {
    //     // Arrange
    //     ProductImageEntity.TryCreate(Guid.NewGuid(), "", 1, out var entity);
    //
    //     var dto = new ProductImageWriteDto
    //     (
    //         ProductId: entity.ProductId,
    //         ImageFileName: "",
    //         DisplayOrder: 2
    //     );
    //
    //     // Act
    //     var result = ProductImageMapper.TryUpdateEntity(dto, entity);
    //
    //     // Assert
    //     result.IsValid.Should().BeTrue();
    //     entity.DisplayOrder.Should().Be(dto.DisplayOrder);
    // }
}