using Catalog.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Catalog.UnitTests.Domain.Entities;

public class ProductEntityTests
{
    [Fact]
    public void TryCreate_ValidParams_ReturnsValidEntity()
    {
        // Arrange
        Guid? brandId = null;
        Guid? categoryId = null;
        var name = "Test Product";
        var description = "Test Description";
        var price = 100.00m;
        var discount = 0m;
        var sku = "1234567890";
        var stock = 10;
        var availability = true;

        // Act
        var result = ProductEntity.TryCreate(brandId, categoryId, name, description,
            price, discount, sku, stock, availability, out var entity);

        // Assert
        result.IsValid.Should().BeTrue();
        entity.Should().NotBeNull();
        entity.BrandId.Should().Be(brandId);
        entity.CategoryId.Should().Be(categoryId);
        entity.Name.Should().Be(name);
        entity.Description.Should().Be(description);
        entity.Price.Should().Be(price);
        entity.Discount.Should().Be(discount);
        entity.SKU.Should().Be(sku);
        entity.Stock.Should().Be(stock);
        entity.Availability.Should().Be(availability);
    }

    [Fact]
    public void Update_ValidParams_ReturnsValidEntity()
    {
        // Arrange
        Guid? brandId = null;
        Guid? categoryId = null;
        var name = "Test Product";
        var description = "Test Description";
        var price = 100.00m;
        var discount = 0m;
        var sku = "1234567890";
        var stock = 10;
        var availability = true;

        ProductEntity.TryCreate(brandId, categoryId, name, description, price,
            discount, sku, stock, availability, out var entity);

        // Act
        Guid? newBrandId = Guid.NewGuid();
        Guid? newCategoryId = Guid.NewGuid();
        var newName = "Updated Product";
        var newDescription = "Updated Description";
        var newPrice = 120.00m;
        var newDiscount = 1m;
        var newSku = "9876543210";
        var newStock = 15;
        var newAvailability = false;

        var result = entity.Update(newBrandId, newCategoryId, newName, newDescription,
            newPrice, newDiscount, newSku, newStock, newAvailability);

        // Assert
        result.IsValid.Should().BeTrue();
        entity.BrandId.Should().Be(newBrandId);
        entity.CategoryId.Should().Be(newCategoryId);
        entity.Name.Should().Be(newName);
        entity.Description.Should().Be(newDescription);
        entity.Price.Should().Be(newPrice);
        entity.Discount.Should().Be(newDiscount);
        entity.SKU.Should().Be(newSku);
        entity.Stock.Should().Be(newStock);
        entity.Availability.Should().Be(newAvailability);
    }
}