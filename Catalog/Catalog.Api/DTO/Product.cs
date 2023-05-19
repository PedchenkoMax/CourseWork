namespace Catalog.Api.DTO;

public record ProductReadDto(
    Guid Id,
    Guid? BrandId,
    Guid? CategoryId,
    string Name,
    string Description,
    decimal Price,
    decimal Discount,
    string SKU,
    int Stock,
    bool Availability,
    BrandReadDto? Brand,
    CategoryReadDto? Category,
    List<ProductImageReadDto>? Images);

public record ProductWriteDto(
    Guid? BrandId,
    Guid? CategoryId,
    string Name,
    string Description,
    decimal Price,
    decimal Discount,
    string SKU,
    int Stock,
    bool Availability);