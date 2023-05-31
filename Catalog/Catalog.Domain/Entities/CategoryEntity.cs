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

    private CategoryEntity()
    {
    }

    private CategoryEntity(Guid? parentCategoryId, string name, string description, string imageFileName)
    {
        Id = Guid.NewGuid();
        ParentCategoryId = parentCategoryId;
        Name = name;
        Description = description;
        ImageFileName = imageFileName;
    }

    public static ValidationResult TryCreate(Guid? parentCategoryId, string name, string description, string imageFileName,
        out CategoryEntity entity)
    {
        entity = new CategoryEntity(parentCategoryId, name, description, imageFileName);

        return new CategoryEntityValidator().Validate(entity);
    }

    public ValidationResult Update(Guid? parentCategoryId, string name, string description, string imageFileName)
    {
        ParentCategoryId = parentCategoryId;
        Name = name;
        Description = description;
        ImageFileName = imageFileName;

        return new CategoryEntityValidator().Validate(this);
    }
}