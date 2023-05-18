using Catalog.Domain.Validators;
using FluentValidation.Results;

namespace Catalog.Domain.Entities;

public class ProductEntity
{
    public Guid Id { get; private set; }
    public Guid? BrandId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public decimal Discount { get; private set; }
    public string SKU { get; private set; }
    public int Stock { get; private set; }
    public bool Availability { get; private set; }
    public BrandEntity? Brand { get; private set; }
    public CategoryEntity? Category { get; private set; }
    public IReadOnlyCollection<ProductImageEntity>? Images => images;
    private List<ProductImageEntity>? images;

    private ProductEntity()
    {
    }

    private ProductEntity(Guid? brandId, Guid? categoryId, string name, string description, decimal price,
        decimal discount, string sku, int stock, bool availability)
    {
        Id = Guid.NewGuid();
        BrandId = brandId;
        CategoryId = categoryId;
        Name = name;
        Description = description;
        Price = price;
        Discount = discount;
        SKU = sku;
        Stock = stock;
        Availability = availability;
    }

    public static ValidationResult TryCreate(Guid? brandId, Guid? categoryId, string name, string description,
        decimal price, decimal discount, string sku, int stock, bool availability, out ProductEntity entity)
    {
        entity = new ProductEntity(brandId, categoryId, name, description, price, discount, sku, stock, availability);

        return new ProductEntityValidator().Validate(entity);
    }

    public ValidationResult Update(Guid? brandId, Guid? categoryId, string name, string description, decimal price,
        decimal discount, string sku, int stock, bool availability)
    {
        BrandId = brandId;
        CategoryId = categoryId;
        Name = name;
        Description = description;
        Price = price;
        Discount = discount;
        SKU = sku;
        Stock = stock;
        Availability = availability;

        return new ProductEntityValidator().Validate(this);
    }
}