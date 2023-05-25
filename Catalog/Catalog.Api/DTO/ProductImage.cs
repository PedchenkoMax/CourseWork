using Catalog.Api.ValidationAttributes;

namespace Catalog.Api.DTO;

public record ProductImageReadDto(
    Guid Id,
    Guid ProductId,
    string ImageUrl,
    int DisplayOrder);

public record ProductImageWriteDto(
    [NonZeroGuid] Guid ProductId,
    string ImageUrl,
    int DisplayOrder);