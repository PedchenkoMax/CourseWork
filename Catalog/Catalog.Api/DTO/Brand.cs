namespace Catalog.Api.DTO;

public class Brand
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public List<Product>? Products { get; set; }
}