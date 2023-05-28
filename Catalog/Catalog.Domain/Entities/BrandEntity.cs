using Catalog.Domain.Validators;
using FluentValidation.Results;

namespace Catalog.Domain.Entities;

public class BrandEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ImageFileName { get; private set; }
    public int DisplayOrder { get; private set; }

    private BrandEntity()
    {
    }

    private BrandEntity(string name, string description, string imageFileName, int displayOrder)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        ImageFileName = imageFileName;
        DisplayOrder = displayOrder;
    }

    public static ValidationResult TryCreate(string name, string description, string imageFileName, int displayOrder,
        out BrandEntity entity)
    {
        entity = new BrandEntity(name, description, imageFileName, displayOrder);

        return new BrandEntityValidator().Validate(entity);
    }

    public ValidationResult Update(string name, string description, string imageFileName, int displayOrder)
    {
        Name = name;
        Description = description;
        ImageFileName = imageFileName;
        DisplayOrder = displayOrder;

        return new BrandEntityValidator().Validate(this);
    }
}