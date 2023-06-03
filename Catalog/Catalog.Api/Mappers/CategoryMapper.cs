using Catalog.Api.DTO;
using Catalog.Api.Mappers.Abstractions;
using Catalog.Api.Services.Abstractions;
using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.Api.Mappers;

/// <summary>
/// Provides mapping between CategoryEntity and Read/Write dto's.
/// </summary>
public class CategoryMapper : ICategoryMapper
{
    private readonly IBlobServiceSettings blobServiceSettings;
    private readonly IImageHandlingSettings imageHandlingSettings;

    public CategoryMapper(IBlobServiceSettings blobServiceSettings, IImageHandlingSettings imageHandlingSettings)
    {
        this.blobServiceSettings = blobServiceSettings;
        this.imageHandlingSettings = imageHandlingSettings;
    }

    /// <summary>
    /// Maps a CategoryEntity to a CategoryReadDto.
    /// </summary>
    /// <param name="entity">The CategoryEntity to map.</param>
    /// <returns>A CategoryReadDto mapped from the given CategoryEntity.</returns>
    public CategoryReadDto MapToDto(CategoryEntity entity)
    {
        var imageUrl = entity.ImageFileName != null
            ? $"{blobServiceSettings.Endpoint}/{blobServiceSettings.CategoryImageBucketName}/{entity.ImageFileName}"
            : $"{blobServiceSettings.Endpoint}/{blobServiceSettings.CategoryImageBucketName}/{imageHandlingSettings.DefaultCategoryImageName}";

        return new CategoryReadDto(
            entity.Id,
            entity.ParentCategoryId,
            entity.Name,
            entity.Description,
            imageUrl);
    }

    /// <summary>
    /// Maps a CategoryWriteDto to a CategoryEntity and validates it.
    /// </summary>
    /// <param name="dto">The CategoryWriteDto to map.</param>
    /// <returns>A tuple with ValidationResult and the created CategoryEntity.</returns>
    public (ValidationResult ValidationResult, CategoryEntity Entity) MapToEntity(CategoryWriteDto dto)
    {
        var validationResult = CategoryEntity.TryCreate(
            dto.ParentCategoryId,
            dto.Name,
            dto.Description,
            null,
            out var entity);

        return (validationResult, entity);
    }
}