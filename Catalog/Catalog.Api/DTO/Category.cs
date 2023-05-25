using Catalog.Api.ValidationAttributes;

namespace Catalog.Api.DTO;

public record CategoryReadDto(
    Guid Id,
    Guid? ParentCategoryId,
    string Name,
    string Description,
    string ImageUrl,
    int DisplayOrder);

public record CategoryWriteDto(
    [NonZeroNullableGuid] Guid? ParentCategoryId,
    string Name,
    string Description,
    string ImageUrl,
    int DisplayOrder);