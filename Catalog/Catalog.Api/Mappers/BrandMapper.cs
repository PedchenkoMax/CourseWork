using Catalog.Api.DTO;
using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.Api.Mappers;

public static class BrandMapper
{
    public static BrandReadDto MapToReadDto(BrandEntity entity)
    {
        return new BrandReadDto(
            Id: entity.Id,
            Name: entity.Name,
            Description: entity.Description,
            ImageUrl: entity.ImageUrl,
            DisplayOrder: entity.DisplayOrder,
            Products: null);
    }

    public static ValidationResult TryCreateEntity(BrandWriteDto writeDto, out BrandEntity entity)
    {
        var validationResult = BrandEntity.TryCreate(
            name: writeDto.Name,
            description: writeDto.Description,
            imageUrl: writeDto.ImageUrl,
            displayOrder: writeDto.DisplayOrder,
            out entity);

        return validationResult;
    }

    public static ValidationResult TryUpdateEntity(BrandEntity entity, BrandWriteDto writeDto)
    {
        var validationResult = entity.Update(
            name: writeDto.Name,
            description: writeDto.Description,
            imageUrl: writeDto.ImageUrl,
            displayOrder: writeDto.DisplayOrder);

        return validationResult;
    }
}