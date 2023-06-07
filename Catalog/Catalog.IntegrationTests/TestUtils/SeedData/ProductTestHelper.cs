using Catalog.Domain.Entities;

namespace Catalog.IntegrationTests.TestUtils.SeedData;

public static class ProductTestHelper
{
    public static ProductEntity Default()
    {
        var validationResult = ProductEntity.TryCreate(
            brandId: null,
            categoryId: null,
            name: "product.Name",
            description: "product.Description",
            price: 1,
            discount: 0,
            sku: "1234567890",
            stock: 0,
            availability: false,
            out var productEntity
        );

        if (!validationResult.IsValid)
            throw new NotImplementedException();

        return productEntity;
    }

    public static ProductEntity Modified()
    {
        var validationResult = ProductEntity.TryCreate(
            brandId: null,
            categoryId: null,
            name: "modified.product.Name",
            description: "modified.product.Description",
            price: 2,
            discount: 1,
            sku: "0987654321",
            stock: 1,
            availability: true,
            out var productEntity
        );

        if (!validationResult.IsValid)
            throw new NotImplementedException();

        return productEntity;
    }

    public static void UpdateTo(this ProductEntity from, ProductEntity to)
    {
        var validationResult = from.Update(
            brandId: to.BrandId,
            categoryId: to.CategoryId,
            name: to.Name,
            description: to.Description,
            price: to.Price,
            discount: to.Discount,
            sku: to.SKU,
            stock: to.Stock,
            availability: to.Availability
        );

        if (!validationResult.IsValid)
            throw new NotImplementedException();
    }
}