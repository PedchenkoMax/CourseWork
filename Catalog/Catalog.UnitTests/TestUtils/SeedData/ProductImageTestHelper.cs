using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.UnitTests.TestUtils.SeedData;

public static class ProductImageTestHelper
{
    public static ProductImageEntity GetEntity(int displayOrder = 1)
    {
        ProductImageEntity.TryCreate(
            productId: Guid.NewGuid(),
            imageFileName: "",
            displayOrder: displayOrder,
            out var productImageEntity);

        return productImageEntity;
    }

    public static ValidationResult UpdateEntity(ProductImageEntity productImageEntity, int displayOrder = 1)
    {
        var validationResult = productImageEntity.Update(displayOrder: displayOrder);

        return validationResult;
    }
}