using Catalog.Api.DTO;
using Catalog.Api.Mappers.Abstractions;
using Catalog.Api.Services.Abstractions;
using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.Api.Mappers;

/// <summary>
/// Provides mapping between BrandEntity and Read/Write dto's.
/// </summary>
public class BrandMapper : IBrandMapper
{
    private readonly IBlobServiceSettings blobServiceSettings;
    private readonly IImageHandlingSettings imageHandlingSettings;

    public BrandMapper(IBlobServiceSettings blobServiceSettings, IImageHandlingSettings imageHandlingSettings)
    {
        this.blobServiceSettings = blobServiceSettings;
        this.imageHandlingSettings = imageHandlingSettings;
    }

    /// <summary>
    /// Maps a BrandEntity to a BrandReadDto.
    /// </summary>
    /// <param name="entity">The BrandEntity to map.</param>
    /// <returns>A BrandReadDto mapped from the given BrandEntity.</returns>
    public BrandReadDto MapToDto(BrandEntity entity)
    {
        var imageUrl = entity.ImageFileName != null
            ? $"{blobServiceSettings.Endpoint}/{blobServiceSettings.BrandImageBucketName}/{entity.ImageFileName}"
            : $"{blobServiceSettings.Endpoint}/{blobServiceSettings.BrandImageBucketName}/{imageHandlingSettings.DefaultBrandImageName}";

        return new BrandReadDto(
            entity.Id,
            entity.Name,
            entity.Description,
            imageUrl);
    }

    /// <summary>
    /// Maps a BrandWriteDto to a BrandEntity and validates it.
    /// </summary>
    /// <param name="dto">The BrandWriteDto to map.</param>
    /// <returns>A tuple with ValidationResult and the created BrandEntity.</returns>
    public (ValidationResult ValidationResult, BrandEntity Entity) MapToEntity(BrandWriteDto dto)
    {
        var validationResult = BrandEntity.TryCreate(
            dto.Name,
            dto.Description,
            null,
            out var entity);

        return (validationResult, entity);
    }
}