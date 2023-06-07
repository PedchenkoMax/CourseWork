namespace Catalog.Api.Services.Abstractions;

public interface IImageHandlingSettings
{
    public int MaxProductImages { get; init; }
    public string DefaultCategoryImageName { get; init; }
    public string DefaultBrandImageName { get; init; }
    public string DefaultProductImageName { get; init; }
}