using Catalog.Domain.Entities;

namespace Catalog.IntegrationTests.SeedData;

public static class BrandTestHelper
{
    public static BrandEntity Default()
    {
        var validationResult = BrandEntity.TryCreate(
            name: "brandDto.Name",
            description: "brandDto.Description",
            imageUrl: "brandDto.ImageUrl",
            displayOrder: 0,
            out var brandEntity
        );

        if (!validationResult.IsValid)
            throw new NotImplementedException();

        return brandEntity;
    }

    public static BrandEntity Modified()
    {
        var validationResult = BrandEntity.TryCreate(
            name: "modified.brandDto.Name",
            description: "modified.brandDto.Description",
            imageUrl: "modified.brandDto.ImageUrl",
            displayOrder: 1,
            out var brandEntity
        );

        if (!validationResult.IsValid)
            throw new NotImplementedException();

        return brandEntity;
    }

    public static void UpdateTo(this BrandEntity from, BrandEntity to)
    {
        var validationResult = from.Update(
            name: to.Name,
            description: to.Description,
            imageUrl: to.ImageUrl,
            displayOrder: to.DisplayOrder
        );

        if (!validationResult.IsValid)
            throw new NotImplementedException();
    }
}