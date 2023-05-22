using Catalog.Api.DTO;
using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.Api.Mappers;

public static class ProductImageMapper
{
    public static ProductImageReadDto MapToReadDto(ProductImageEntity entity)
    {
        return new ProductImageReadDto(
            Id: entity.Id,
            ProductId: entity.ProductId,
            ImageUrl: entity.ImageUrl,
            DisplayOrder: entity.DisplayOrder,
            Product: entity.Product != null ? ProductMapper.MapToReadDto(entity.Product) : null);
    }

    public static ValidationResult TryCreateEntity(ProductImageWriteDto writeDto, out ProductImageEntity entity)
    {
        var validationResult = ProductImageEntity.TryCreate(
            productId: writeDto.ProductId,
            imageUrl: writeDto.ImageUrl,
            displayOrder: writeDto.DisplayOrder,
            out entity);

        return validationResult;
    }

    public static ValidationResult TryUpdateEntity(ProductImageWriteDto writeDto, ProductImageEntity entity)
    {
        var validationResult = entity.Update(displayOrder: writeDto.DisplayOrder);

        return validationResult;
    }
}