using Catalog.Domain.Entities;

namespace Catalog.IntegrationTests.TestUtils.SeedData;

public static class BrandTestHelper
{
    public static BrandEntity Default()
    {
        var validationResult = BrandEntity.TryCreate(
            name: "brandDto.Name",
            description: "brandDto.Description",
            imageFileName: "brandDto.ImageFileName",
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
            imageFileName: "modified.brandDto.ImageFileName",
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
            imageFileName: to.ImageFileName
        );

        if (!validationResult.IsValid)
            throw new NotImplementedException();
    }
}