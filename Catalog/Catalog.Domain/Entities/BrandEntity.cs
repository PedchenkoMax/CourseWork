namespace Catalog.Domain.Entities;

public class BrandEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ImageUrl { get; private set; }
    public int DisplayOrder { get; private set; }
    public ICollection<ProductEntity>? Products { get; private set; }

    private BrandEntity()
    {
    }

    private BrandEntity(string name, string description, string imageUrl, int displayOrder)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
    }

}