using Catalog.Api.DTO;
using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.Api.Mappers.Abstractions;

public interface IProductMapper
{
    ProductReadDto MapToDto(ProductEntity entity);
    (ValidationResult ValidationResult, ProductEntity Entity) MapToEntity(ProductWriteDto dto);
}