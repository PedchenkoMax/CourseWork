using Catalog.Domain.Entities;

namespace Catalog.IntegrationTests.TestUtils.SeedData;

public static class CategoryTestHelper
{
    public static CategoryEntity Default()
    {
        var validationResult = CategoryEntity.TryCreate(
            parentCategoryId: null,
            name: "category.Name",
            description: "category.Description",
            imageFileName: "category.ImageFileName",
            displayOrder: 0,
            out var categoryEntity
        );

        if (!validationResult.IsValid)
            throw new NotImplementedException();

        return categoryEntity;
    }

    public static CategoryEntity Modified()
    {
        var validationResult = CategoryEntity.TryCreate(
            parentCategoryId: null,
            name: "modified.category.Name",
            description: "modified.category.Description",
            imageFileName: "modified.category.ImageFileName",
            displayOrder: 1,
            out var categoryEntity
        );

        if (!validationResult.IsValid)
            throw new NotImplementedException();

        return categoryEntity;
    }

    public static void UpdateTo(this CategoryEntity from, CategoryEntity to)
    {
        var validationResult = from.Update(
            parentCategoryId: to.ParentCategoryId,
            name: to.Name,
            description: to.Description,
            imageFileName: to.ImageFileName,
            displayOrder: to.DisplayOrder
        );

        if (!validationResult.IsValid)
            throw new NotImplementedException();
    }

    public static CategoryEntity ChildOf(CategoryEntity parentCategory)
    {
        var validationResult = CategoryEntity.TryCreate(
            parentCategoryId: parentCategory.Id,
            name: $"childOf.{parentCategory.Name}",
            description: $"childOf.{parentCategory.Description}",
            imageFileName: $"childOf.{parentCategory.ImageFileName}",
            displayOrder: parentCategory.DisplayOrder,
            out var categoryEntity
        );

        if (!validationResult.IsValid)
            throw new NotImplementedException();

        return categoryEntity;
    }
}