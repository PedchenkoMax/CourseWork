using Catalog.Domain.Validators;
using FluentValidation.Results;

namespace Catalog.Domain.Entities;

public class ProductImageEntity
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string ImageUrl { get; private set; }
    public int DisplayOrder { get; private set; }

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

    public static ValidationResult TryCreate(Guid productId, string imageUrl, int displayOrder,
        out ProductImageEntity entity)
    {
        entity = new ProductImageEntity(productId, imageUrl, displayOrder);

        return new ProductImageEntityValidator().Validate(entity);
    }

    public ValidationResult Update(int displayOrder)
    {
        DisplayOrder = displayOrder;

        return new ProductImageEntityValidator().Validate(this);
    }
}