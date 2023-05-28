using Catalog.Domain.Validators;
using FluentValidation.Results;

namespace Catalog.Domain.Entities;

public class CategoryEntity
{
    public Guid Id { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ImageFileName { get; private set; }
    public int DisplayOrder { get; private set; }

    private CategoryEntity()
    {
    }

    private CategoryEntity(Guid? parentCategoryId, string name, string description, string imageFileName, int displayOrder)
    {
        Id = Guid.NewGuid();
        ParentCategoryId = parentCategoryId;
        Name = name;
        Description = description;
        ImageFileName = imageFileName;
        DisplayOrder = displayOrder;
    }

    public static ValidationResult TryCreate(Guid? parentCategoryId, string name, string description, string imageFileName,
        int displayOrder, out CategoryEntity entity)
    {
        entity = new CategoryEntity(parentCategoryId, name, description, imageFileName, displayOrder);

        return new CategoryEntityValidator().Validate(entity);
    }

    public ValidationResult Update(Guid? parentCategoryId, string name, string description, string imageFileName, int displayOrder)
    {
        ParentCategoryId = parentCategoryId;
        Name = name;
        Description = description;
        ImageFileName = imageFileName;
        DisplayOrder = displayOrder;

        return new CategoryEntityValidator().Validate(this);
    }
}