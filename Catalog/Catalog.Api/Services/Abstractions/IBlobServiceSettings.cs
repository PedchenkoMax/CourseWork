namespace Catalog.Api.Services.Abstractions;

public interface IBlobServiceSettings
{
    public string Endpoint { get; init; }
    public string BrandImageBucketName { get; init; }
    public string CategoryImageBucketName { get; init; }
    public string ProductImageBucketName { get; init; }
}