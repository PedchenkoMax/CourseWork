namespace Catalog.Api.DTO;

public class Product
{
    public Guid Id { get; set; }
    public Guid BrandId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public string SKU { get; set; }
    public int Stock { get; set; }
    public bool Availability { get; set; }
    public Brand? Brand { get; set; }
    public Category? Category { get; set; }
    public List<ProductImage>? Images { get; set; }
}