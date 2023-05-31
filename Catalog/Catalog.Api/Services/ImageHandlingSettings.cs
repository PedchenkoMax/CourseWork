using Catalog.Api.Services.Abstractions;

namespace Catalog.Api.Services;

public class ImageHandlingSettings : IImageHandlingSettings
{
    public int MaxProductImages { get; init; }
    public string DefaultCategoryImageName { get; init; } = null!;
    public string DefaultBrandImageName { get; init; } = null!;
    public string DefaultProductImageName { get; init; } = null!;
}