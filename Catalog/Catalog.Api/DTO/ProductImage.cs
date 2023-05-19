namespace Catalog.Api.DTO;

public record ProductImageReadDto(
    Guid Id,
    Guid ProductId,
    string ImageUrl,
    int DisplayOrder,
    ProductReadDto? Product);

public record ProductImageWriteDto(
    Guid ProductId,
    string ImageUrl,
    int DisplayOrder,
    ProductReadDto? Product);