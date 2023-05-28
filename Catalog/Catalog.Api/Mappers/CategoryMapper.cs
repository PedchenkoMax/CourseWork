using Catalog.Api.DTO;
using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.Api.Mappers;

public static class CategoryMapper
{
    public static CategoryReadDto MapToReadDto(CategoryEntity entity)
    {
        return new CategoryReadDto(
            Id: entity.Id,
            ParentCategoryId: entity.ParentCategoryId,
            Name: entity.Name,
            Description: entity.Description,
            ImageUrl: entity.ImageFileName,
            DisplayOrder: entity.DisplayOrder);
    }

    public static ValidationResult TryCreateEntity(CategoryWriteDto writeDto, out CategoryEntity entity)
    {
        var validationResult = CategoryEntity.TryCreate(
            parentCategoryId: writeDto.ParentCategoryId,
            name: writeDto.Name,
            description: writeDto.Description,
            imageFileName: writeDto.ImageUrl,
            displayOrder: writeDto.DisplayOrder,
            out entity);

        return validationResult;
    }

    public static ValidationResult TryUpdateEntity(CategoryWriteDto writeDto, CategoryEntity entity)
    {
        var validationResult = entity.Update(
            parentCategoryId: writeDto.ParentCategoryId,
            name: writeDto.Name,
            description: writeDto.Description,
            imageFileName: writeDto.ImageUrl,
            displayOrder: writeDto.DisplayOrder);

        return validationResult;
    }
}