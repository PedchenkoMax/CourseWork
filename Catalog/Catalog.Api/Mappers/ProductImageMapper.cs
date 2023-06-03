using Catalog.Api.DTO;
using Catalog.Api.Mappers.Abstractions;
using Catalog.Api.Services.Abstractions;
using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.Api.Mappers;

/// <summary>
/// Provides mapping between ProductImageEntity and Read/Update dto's.
/// </summary>
public class ProductImageMapper : IProductImageMapper
{
    private readonly IBlobServiceSettings blobServiceSettings;

    public ProductImageMapper(IBlobServiceSettings blobServiceSettings)
    {
        this.blobServiceSettings = blobServiceSettings;
    }

    /// <summary>
    /// Maps a ProductImageEntity to a ProductImageReadDto.
    /// </summary>
    /// <param name="entity">The ProductImageEntity to map.</param>
    /// <returns>A ProductImageReadDto mapped from the given ProductImageEntity.</returns>
    public ProductImageReadDto MapToDto(ProductImageEntity entity)
    {
        var imageUrl = $"{blobServiceSettings.Endpoint}/{blobServiceSettings.ProductImageBucketName}/{entity.ImageFileName}";

        return new ProductImageReadDto(
            entity.Id,
            entity.ProductId,
            imageUrl,
            entity.DisplayOrder);
    }

    /// <summary>
    /// Creates a ProductImageEntity based on given productId, fileName, and displayOrder, and validates it.
    /// </summary>
    /// <param name="productId">The ProductId to create the ProductImageEntity.</param>
    /// <param name="fileName">The fileName of the image for the ProductImageEntity.</param>
    /// <param name="displayOrder">The display order of the image for the ProductImageEntity.</param>
    /// <returns>A tuple with ValidationResult and the created ProductImageEntity.</returns>
    public (ValidationResult ValidationResult, ProductImageEntity Entity) MapToEntity(Guid productId, string fileName, int displayOrder)
    {
        var validationResult = ProductImageEntity.TryCreate(
            productId,
            fileName,
            displayOrder,
            out var entity);

        return (validationResult, entity);
    }
}