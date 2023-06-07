using Catalog.Api.DTO;
using Catalog.Api.Mappers.Abstractions;
using Catalog.Api.Services.Abstractions;
using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.Api.Mappers;

/// <summary>
/// Provides mapping between ProductEntity and Read/Write dto's.
/// </summary>
public class ProductMapper : IProductMapper
{
    private readonly IBrandMapper brandMapper;
    private readonly ICategoryMapper categoryMapper;
    private readonly IProductImageMapper productImageMapper;
    private readonly List<ProductImageReadDto> defaultImage; // TODO: probably there is better solution

    public ProductMapper(IBrandMapper brandMapper, ICategoryMapper categoryMapper, IProductImageMapper productImageMapper,
        IBlobServiceSettings blobServiceSettings, IImageHandlingSettings imageHandlingSettings)
    {
        this.brandMapper = brandMapper;
        this.categoryMapper = categoryMapper;
        this.productImageMapper = productImageMapper;

        defaultImage = new List<ProductImageReadDto>
        {
            new ProductImageReadDto(Id: Guid.NewGuid(), ProductId: Guid.NewGuid(), DisplayOrder: 1,
                ImageUrl: $"{blobServiceSettings.Endpoint}/{blobServiceSettings.ProductImageBucketName}/{imageHandlingSettings.DefaultProductImageName}")
        };
    }

    /// <summary>
    /// Maps a ProductEntity to a ProductReadDto.
    /// </summary>
    /// <param name="entity">The ProductEntity to map.</param>
    /// <returns>A ProductReadDto mapped from the given ProductEntity.</returns>
    public ProductReadDto MapToDto(ProductEntity entity)
    {
        var brandReadDto = entity.Brand != null
            ? brandMapper.MapToDto(entity.Brand)
            : null;

        var categoryReadDto = entity.Category != null
            ? categoryMapper.MapToDto(entity.Category)
            : null;

        var productImagesReadDto = entity.Images.Count != 0
            ? entity.Images.Select(productImageMapper.MapToDto).ToList()
            : defaultImage;

        return new ProductReadDto(
            entity.Id,
            entity.BrandId,
            entity.CategoryId,
            entity.Slug,
            entity.Name,
            entity.Description,
            entity.Price,
            entity.Discount,
            entity.SKU,
            entity.Stock,
            entity.Availability,
            brandReadDto,
            categoryReadDto,
            productImagesReadDto);
    }

    /// <summary>
    /// Maps a ProductWriteDto to a ProductEntity and validates it.
    /// </summary>
    /// <param name="dto">The ProductWriteDto to map.</param>
    /// <returns>A tuple with ValidationResult and the created ProductEntity.</returns>
    public (ValidationResult ValidationResult, ProductEntity Entity) MapToEntity(ProductWriteDto dto)
    {
        var validationResult = ProductEntity.TryCreate(
            dto.BrandId,
            dto.CategoryId,
            dto.Name,
            dto.Description,
            dto.Price,
            dto.Discount,
            dto.SKU,
            dto.Stock,
            dto.Availability,
            out var entity);

        return (validationResult, entity);
    }
}