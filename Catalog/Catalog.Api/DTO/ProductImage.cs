namespace Catalog.Api.DTO;

public record ProductImageReadDto(
    Guid Id,
    Guid ProductId,
    string ImageUrl,
    int DisplayOrder);

public record ProductImageWriteDto(IFormFile ImageFile); // TODO: maybe add attributes to validate

public record ProductImageUpdateDto(int DisplayOrder);