using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.UnitTests.TestUtils.SeedData;

public static class ProductTestHelper
{
    public static ProductEntity GetEntity(string name = "name", string description = "description", int price = 150,
        decimal discount = 0.1m, string sku = "1234567890", int stock = 1, bool availability = true)
    {
        ProductEntity.TryCreate(
            brandId: Guid.NewGuid(),
            categoryId: Guid.NewGuid(),
            name: name,
            description: description,
            price: price,
            discount: discount,
            sku: sku,
            stock: stock,
            availability: availability,
            out var categoryEntity);

        return categoryEntity;
    }

    public static ValidationResult UpdateEntity(ProductEntity productEntity, string name = "name", string description = "description",
        int price = 150, decimal discount = 0.1m, string sku = "1234567890", int stock = 1, bool availability = true)
    {
        var result = productEntity.Update(
            brandId: Guid.NewGuid(),
            categoryId: Guid.NewGuid(),
            name: name,
            description: description,
            price: price,
            discount: discount,
            sku: sku,
            stock: stock,
            availability: availability);

        return result;
    }
}