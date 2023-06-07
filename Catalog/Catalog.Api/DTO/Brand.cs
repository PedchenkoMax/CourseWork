namespace Catalog.Api.DTO;

public record BrandReadDto(
    Guid Id,
    string Name,
    string Description,
    string ImageUrl);

public record BrandWriteDto(
    string Name,
    string Description);
    
public record BrandImageUpdateDto(IFormFile ImageFile);