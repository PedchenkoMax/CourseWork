using Catalog.Api.DTO;
using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.Api.Mappers;

public static class ProductMapper
{
    public static ProductReadDto MapToReadDto(ProductEntity entity)
    {
        return new ProductReadDto(
            Id: entity.Id,
            BrandId: entity.BrandId,
            CategoryId: entity.CategoryId,
            Name: entity.Name,
            Description: entity.Description,
            Price: entity.Price,
            Discount: entity.Discount,
            SKU: entity.SKU,
            Stock: entity.Stock,
            Availability: entity.Availability,
            Brand: entity.Brand != null ? BrandMapper.MapToReadDto(entity.Brand) : null,
            Category: entity.Category != null ? CategoryMapper.MapToReadDto(entity.Category) : null,
            Images: entity.Images?.Select(imageEntity => ProductImageMapper.MapToReadDto(imageEntity)).ToList());
    }

    public static ValidationResult TryCreateEntity(ProductWriteDto writeDto, out ProductEntity entity)
    {
        var validationResult = ProductEntity.TryCreate(
            brandId: writeDto.BrandId,
            categoryId: writeDto.CategoryId,
            name: writeDto.Name,
            description: writeDto.Description,
            price: writeDto.Price,
            discount: writeDto.Discount,
            sku: writeDto.SKU,
            stock: writeDto.Stock,
            availability: writeDto.Availability,
            out entity);

        return validationResult;
    }

    public static ValidationResult TryUpdateEntity(ProductWriteDto writeDto, ProductEntity entity)
    {
        var validationResult = entity.Update(
            brandId: writeDto.BrandId,
            categoryId: writeDto.CategoryId,
            name: writeDto.Name,
            description: writeDto.Description,
            price: writeDto.Price,
            discount: writeDto.Discount,
            sku: writeDto.SKU,
            stock: writeDto.Stock,
            availability: writeDto.Availability);

        return validationResult;
    }
}