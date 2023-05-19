namespace Catalog.Api.DTO;

public record BrandReadDto(
    Guid Id,
    string Name,
    string Description,
    string ImageUrl,
    int DisplayOrder,
    List<ProductReadDto>? Products);

public record BrandWriteDto(
    string Name,
    string Description,
    string ImageUrl,
    int DisplayOrder);