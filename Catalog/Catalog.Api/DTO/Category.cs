namespace Catalog.Api.DTO;

public class Category
{
    public Guid Id { get; set; }
    public Guid ParentCategoryId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public Category? ParentCategory { get; set; }
    public List<Product>? Products { get; set; }
}