using Catalog.Api.ValidationAttributes;

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
    [NonZeroNullableGuid] Guid? BrandId,
    [NonZeroNullableGuid] Guid? CategoryId,
    string Name,
    string Description,
    decimal Price,
    decimal Discount,
    string SKU,
    int Stock,
    bool Availability);