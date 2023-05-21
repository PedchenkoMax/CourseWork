using Catalog.Domain.Entities;

namespace Catalog.IntegrationTests.SeedData;

public static class ProductImageTestHelper
{
    public static ProductImageEntity Default(ProductEntity productEntity)
    {
        var validationResult = ProductImageEntity.TryCreate(
            productId: productEntity.Id,
            imageUrl: "productImage.ImageUrl",
            displayOrder: 0,
            out var productImageEntity
        );

        if (!validationResult.IsValid)
            throw new NotImplementedException();

        return productImageEntity;
    }

    public static ProductImageEntity Modified(ProductEntity productEntity)
    {
        var validationResult = ProductImageEntity.TryCreate(
            productId: productEntity.Id,
            imageUrl: "modified.productImage.ImageUrl",
            displayOrder: 1,
            out var productImageEntity
        );

        if (!validationResult.IsValid)
            throw new NotImplementedException();

        return productImageEntity;
    }

    public static void UpdateTo(this ProductImageEntity from, ProductImageEntity to)
    {
        var validationResult = from.Update(displayOrder: to.DisplayOrder);

        if (!validationResult.IsValid)
            throw new NotImplementedException();
    }
}