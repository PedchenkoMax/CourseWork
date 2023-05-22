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
            Brand: null,
            Category: null,
            Images: null);
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

    public static ValidationResult TryUpdateEntity(ProductEntity entity, ProductWriteDto writeDto)
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