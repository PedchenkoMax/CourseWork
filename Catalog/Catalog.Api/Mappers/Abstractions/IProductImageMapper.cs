using Catalog.Api.DTO;
using Catalog.Domain.Entities;
using FluentValidation.Results;

namespace Catalog.Api.Mappers.Abstractions;

public interface IProductImageMapper
{
    ProductImageReadDto MapToDto(ProductImageEntity entity);
    (ValidationResult ValidationResult, ProductImageEntity? Entity) MapToEntity(Guid productId, string fileName, int displayOrder);
}