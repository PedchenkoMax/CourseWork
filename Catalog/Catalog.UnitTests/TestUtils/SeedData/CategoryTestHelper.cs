using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.UnitTests.TestUtils.SeedData;

public static class CategoryTestHelper
{
    public static CategoryEntity GetEntity(Guid? parentCategoryId = null, string name = "name",
        string description = "description", string imageUrl = "")
    {
        CategoryEntity.TryCreate(
            parentCategoryId: parentCategoryId,
            name: name,
            description: description,
            imageFileName: imageUrl,
            out var categoryEntity);

        return categoryEntity;
    }

    public static ValidationResult UpdateEntity(CategoryEntity categoryEntity, Guid? parentCategoryId = null,
        string name = "name", string description = "description", string imageUrl = "")
    {
        var validationResult = categoryEntity.Update(
            parentCategoryId: parentCategoryId,
            name: name,
            description: description,
            imageFileName: imageUrl);

        return validationResult;
    }
}