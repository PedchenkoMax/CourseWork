using Catalog.Api.DTO;
using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.Api.Mappers.Abstractions;

public interface IBrandMapper
{
    BrandReadDto MapToDto(BrandEntity entity);
    (ValidationResult ValidationResult, BrandEntity Entity) MapToEntity(BrandWriteDto dto);
}