namespace Catalog.Domain.Entities;

public class ProductImageEntity
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string ImageUrl { get; private set; }
    public int DisplayOrder { get; private set; }
    public ProductEntity? Product { get; private set; }

    private ProductImageEntity()
    {
    }

    private ProductImageEntity(Guid productId, string imageUrl, int displayOrder)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
    }
}