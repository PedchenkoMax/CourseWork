using Catalog.Domain.Validators;
using FluentValidation.Results;

namespace Catalog.Domain.Entities;

public class ProductImageEntity
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string ImageFileName { get; private set; }
    public int DisplayOrder { get; private set; }

    private ProductImageEntity()
    {
    }

    private ProductImageEntity(Guid productId, string imageFileName, int displayOrder)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ImageFileName = imageFileName;
        DisplayOrder = displayOrder;
    }

    public static ValidationResult TryCreate(Guid productId, string imageFileName, int displayOrder,
        out ProductImageEntity entity)
    {
        entity = new ProductImageEntity(productId, imageFileName, displayOrder);

        return new ProductImageEntityValidator().Validate(entity);
    }

    public ValidationResult Update(int displayOrder)
    {
        DisplayOrder = displayOrder;

        return new ProductImageEntityValidator().Validate(this);
    }
}