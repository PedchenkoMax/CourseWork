using Catalog.Domain.Validators;
using FluentValidation.Results;

namespace Catalog.Domain.Entities;

public class CategoryEntity
{
    public Guid Id { get; private set; }
    public Guid ParentCategoryId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ImageUrl { get; private set; }
    public int DisplayOrder { get; private set; }
    public CategoryEntity? ParentCategory { get; private set; }
    public IReadOnlyCollection<ProductEntity>? Products => products;
    private List<ProductEntity>? products;

    private CategoryEntity()
    {
    }

    private CategoryEntity(Guid parentCategoryId, string name, string description, string imageUrl, int displayOrder)
    {
        Id = Guid.NewGuid();
        ParentCategoryId = parentCategoryId;
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
    }

    public static ValidationResult TryCreate(Guid parentCategoryId, string name, string description, string imageUrl,
        int displayOrder, out CategoryEntity entity)
    {
        entity = new CategoryEntity(parentCategoryId, name, description, imageUrl, displayOrder);

        return new CategoryEntityValidator().Validate(entity);
    }
    
    public ValidationResult Update(Guid parentCategoryId, string name, string description, string imageUrl, int displayOrder)
    {
        ParentCategoryId = parentCategoryId;
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;

        return new CategoryEntityValidator().Validate(this);
    }
}