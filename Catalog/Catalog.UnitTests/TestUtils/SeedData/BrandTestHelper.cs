using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.UnitTests.TestUtils.SeedData;

public static class BrandTestHelper
{
    public static BrandEntity GetEntity(string name = "name",
        string description = "description",
        string imageUrl = "",
        int displayOrder = 1)
    {
        BrandEntity.TryCreate(
            name: name,
            description: description,
            imageFileName: imageUrl,
            displayOrder: displayOrder,
            out var brandEntity);

        return brandEntity;
    }

    public static ValidationResult UpdateEntity(BrandEntity brandEntity,
        string name = "name",
        string description = "description",
        string imageUrl = "",
        int displayOrder = 1)
    {
        var validationResult = brandEntity.Update(
            name: name,
            description: description,
            imageFileName: imageUrl,
            displayOrder: displayOrder);

        return validationResult;
    }
}