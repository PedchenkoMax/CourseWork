namespace Catalog.Api.Services.Abstractions;

public interface IBlobServiceSettings
{
    public string Endpoint { get; init; }
    public string BrandImageBucketName { get; init; }
    public string CategoryImageBucketName { get; init; }
    public string ProductImageBucketName { get; init; }
    public int MaxProductImages { get; init; }
    public string DefaultCategoryImageName { get; init; }
    public string DefaultBrandImageName { get; init; }
    public string DefaultProductImageName { get; init; }
}