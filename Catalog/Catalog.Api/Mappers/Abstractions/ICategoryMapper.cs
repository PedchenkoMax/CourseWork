using Catalog.Api.DTO;
using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.Api.Mappers.Abstractions;

public interface ICategoryMapper
{
    CategoryReadDto MapToDto(CategoryEntity entity);
    (ValidationResult ValidationResult, CategoryEntity Entity) MapToEntity(CategoryWriteDto dto);
}