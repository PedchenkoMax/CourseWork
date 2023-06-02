using System.Text.RegularExpressions;
using Catalog.Domain.Validators;
using FluentValidation.Results;

namespace Catalog.Domain.Entities;

public class ProductEntity
{
    public Guid Id { get; private set; }
    public Guid? BrandId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public string Slug { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public decimal Discount { get; private set; }
    public string SKU { get; private set; }
    public int Stock { get; private set; }
    public bool Availability { get; private set; }
    public BrandEntity? Brand { get; set; }
    public CategoryEntity? Category { get; set; }
    public List<ProductImageEntity> Images { get; set; } = new();

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

        var validationResult = new ProductEntityValidator().Validate(entity);

        if (validationResult.IsValid)
            entity.Slug = entity.GenerateSlug(name, sku);

        return validationResult;
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
        Slug = GenerateSlug(name, sku);

        return new ProductEntityValidator().Validate(this);
    }

    private string GenerateSlug(string name, string sku)
    {
        // Convert to lower case
        var slug = name.ToLowerInvariant();

        // Replace spaces with hyphens
        slug = Regex.Replace(slug, @"\s", "-", RegexOptions.Compiled);

        // Remove invalid characters, allow only letters, numbers, hyphen and underscore
        slug = Regex.Replace(slug, @"[^а-яА-Яa-zA-Z0-9\s-_]", "", RegexOptions.Compiled);

        // Trim hyphens and underscores from both ends
        slug = slug.Trim('-', '_');

        // Replace multiple hyphens or underscores with one
        slug = Regex.Replace(slug, @"([-_]){2,}", "$1", RegexOptions.Compiled);

        // Append SKU to the slug to ensure uniqueness
        slug = $"{slug}-{sku}";

        return slug;
    }
}