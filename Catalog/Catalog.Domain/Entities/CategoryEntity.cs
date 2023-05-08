namespace Catalog.Domain.Entities;

public class CategoryEntity
{
    public Guid Id { get; private set; }
    public Guid ParentCategoryId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ImageUrl { get; private set; }
    public int DisplayOrder { get; private set; }
    public ICollection<ProductEntity>? Products { get; private set; }
    public CategoryEntity? ParentCategory { get; private set; }

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
}