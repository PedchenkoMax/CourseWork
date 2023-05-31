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
            ImageUrl: entity.ImageFileName);
    }

    public static ValidationResult TryCreateEntity(BrandWriteDto writeDto, out BrandEntity entity)
    {
        var validationResult = BrandEntity.TryCreate(
            name: writeDto.Name,
            description: writeDto.Description,
            imageFileName: writeDto.ImageUrl,
            out entity);

        return validationResult;
    }

    public static ValidationResult TryUpdateEntity(BrandWriteDto writeDto, BrandEntity entity)
    {
        var validationResult = entity.Update(
            name: writeDto.Name,
            description: writeDto.Description,
            imageFileName: writeDto.ImageUrl);

        return validationResult;
    }
}