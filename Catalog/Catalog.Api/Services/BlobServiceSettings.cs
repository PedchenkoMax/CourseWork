using Catalog.Api.Services.Abstractions;

namespace Catalog.Api.Services;

public class BlobServiceSettings : IBlobServiceSettings
{
    public string Endpoint { get; init; } = null!;
    public string BrandImageBucketName { get; init; } = null!;
    public string CategoryImageBucketName { get; init; } = null!;
    public string ProductImageBucketName { get; init; } = null!;
}