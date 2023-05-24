using Catalog.Api.DTO;
using Catalog.Api.Mappers;
using Catalog.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Catalog.UnitTests.Api.Mappers;

public class ProductMapperTests
{
    [Fact]
    public void MapToReadDto_ValidEntity_ReturnsValidDto()
    {
        // Arrange
        ProductEntity.TryCreate(
            brandId: null,
            categoryId: null,
            name: "Test Product",
            description: "Test Description",
            price: 100.00m,
            discount: 0m,
            sku: "1234567890",
            stock: 10,
            availability: true,
            out var entity
        );

        // Act
        var dto = ProductMapper.MapToReadDto(entity);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(entity.Id);
        dto.BrandId.Should().Be(entity.BrandId);
        dto.CategoryId.Should().Be(entity.CategoryId);
        dto.Name.Should().Be(entity.Name);
        dto.Description.Should().Be(entity.Description);
        dto.Price.Should().Be(entity.Price);
        dto.Discount.Should().Be(entity.Discount);
        dto.SKU.Should().Be(entity.SKU);
        dto.Stock.Should().Be(entity.Stock);
        dto.Availability.Should().Be(entity.Availability);
    }

    [Fact]
    public void TryCreateEntity_ValidDto_ReturnsValidEntity()
    {
        // Arrange
        var dto = new ProductWriteDto
        (
            BrandId: null,
            CategoryId: null,
            Name: "Test Product",
            Description: "Test Description",
            Price: 100.00m,
            Discount: 0m,
            SKU: "1234567890",
            Stock: 10,
            Availability: true
        );

        // Act
        var result = ProductMapper.TryCreateEntity(dto, out var entity);

        // Assert
        result.IsValid.Should().BeTrue();
        entity.Should().NotBeNull();
        entity.BrandId.Should().Be(dto.BrandId);
        entity.CategoryId.Should().Be(dto.CategoryId);
        entity.Name.Should().Be(dto.Name);
        entity.Description.Should().Be(dto.Description);
        entity.Price.Should().Be(dto.Price);
        entity.Discount.Should().Be(dto.Discount);
        entity.SKU.Should().Be(dto.SKU);
        entity.Stock.Should().Be(dto.Stock);
        entity.Availability.Should().Be(dto.Availability);
    }

    [Fact]
    public void TryUpdateEntity_ValidDto_ReturnsValidEntity()
    {
        // Arrange
        ProductEntity.TryCreate(Guid.NewGuid(), Guid.NewGuid(), "Test Product", "Test Description", 
            100m, 0m, "1234567890", 10, false, out var entity);
        
        var updatedDto = new ProductWriteDto
        (
            BrandId: Guid.NewGuid(), 
            CategoryId: Guid.NewGuid(),
            Name: "Updated Test Product",
            Description: "Updated Test Description",
            Price: 120.00m,
            Discount: 1m,
            SKU: "9876543210",
            Stock: 15,
            Availability: false
        );

        // Act
        var result = ProductMapper.TryUpdateEntity(updatedDto, entity);

        // Assert
        result.IsValid.Should().BeTrue();
        entity.BrandId.Should().Be(updatedDto.BrandId);
        entity.CategoryId.Should().Be(updatedDto.CategoryId);
        entity.Name.Should().Be(updatedDto.Name);
        entity.Description.Should().Be(updatedDto.Description);
        entity.Price.Should().Be(updatedDto.Price);
        entity.Discount.Should().Be(updatedDto.Discount);
        entity.SKU.Should().Be(updatedDto.SKU);
        entity.Stock.Should().Be(updatedDto.Stock);
        entity.Availability.Should().Be(updatedDto.Availability);
    }
}
