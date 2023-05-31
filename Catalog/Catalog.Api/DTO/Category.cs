using Catalog.Api.ValidationAttributes;

namespace Catalog.Api.DTO;

public record CategoryReadDto(
    Guid Id,
    Guid? ParentCategoryId,
    string Name,
    string Description,
    string ImageUrl);

public record CategoryWriteDto(
    [NonZeroNullableGuid] Guid? ParentCategoryId,
    string Name,
    string Description,
    IFormFile? ImageFile);