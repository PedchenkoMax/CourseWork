using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.UnitTests.SeedData;

public static class CategoryTestHelper
{
    public static CategoryEntity GetEntity(Guid? parentCategoryId = null, string name = "name",
        string description = "description", string imageUrl = "", int displayOrder = 1)
    {
        CategoryEntity.TryCreate(
            parentCategoryId: parentCategoryId,
            name: name,
            description: description,
            imageUrl: imageUrl,
            displayOrder: displayOrder,
            out var categoryEntity);

        return categoryEntity;
    }

    public static ValidationResult UpdateEntity(CategoryEntity categoryEntity, Guid? parentCategoryId = null,
        string name = "name", string description = "description", string imageUrl = "", int displayOrder = 1)
    {
        var validationResult = categoryEntity.Update(
            parentCategoryId: parentCategoryId,
            name: name,
            description: description,
            imageUrl: imageUrl,
            displayOrder: displayOrder);

        return validationResult;
    }
}