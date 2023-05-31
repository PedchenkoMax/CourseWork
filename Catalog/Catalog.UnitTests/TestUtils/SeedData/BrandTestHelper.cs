using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.UnitTests.TestUtils.SeedData;

public static class BrandTestHelper
{
    public static BrandEntity GetEntity(string name = "name",
        string description = "description",
        string imageUrl = "")
    {
        BrandEntity.TryCreate(
            name: name,
            description: description,
            imageFileName: imageUrl,
            out var brandEntity);

        return brandEntity;
    }

    public static ValidationResult UpdateEntity(BrandEntity brandEntity,
        string name = "name",
        string description = "description",
        string imageUrl = "")
    {
        var validationResult = brandEntity.Update(
            name: name,
            description: description,
            imageFileName: imageUrl);

        return validationResult;
    }
}