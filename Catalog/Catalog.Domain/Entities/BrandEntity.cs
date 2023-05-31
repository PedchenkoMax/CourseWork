using Catalog.Domain.Validators;
using FluentValidation.Results;

namespace Catalog.Domain.Entities;

public class BrandEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ImageFileName { get; private set; }

    private BrandEntity()
    {
    }

    private BrandEntity(string name, string description, string imageFileName)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        ImageFileName = imageFileName;
    }

    public static ValidationResult TryCreate(string name, string description, string imageFileName, out BrandEntity entity)
    {
        entity = new BrandEntity(name, description, imageFileName);

        return new BrandEntityValidator().Validate(entity);
    }

    public ValidationResult Update(string name, string description, string imageFileName)
    {
        Name = name;
        Description = description;
        ImageFileName = imageFileName;

        return new BrandEntityValidator().Validate(this);
    }
}