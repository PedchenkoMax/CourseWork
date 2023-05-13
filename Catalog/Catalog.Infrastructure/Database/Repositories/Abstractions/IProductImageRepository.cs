using Catalog.Domain.Entities;

namespace Catalog.Infrastructure.Database.Repositories.Abstractions;

public interface IProductImageRepository
{
    Task<ProductImageEntity?> GetByProductIdAsync(Guid id);
    Task<ProductImageEntity?> GetByIdAsync(Guid id);
    Task<bool> UpdateAsync(ProductImageEntity productImage);
    Task<bool> RemoveByIdAsync(Guid id);
    Task<bool> AddAsync(ProductImageEntity productImage);
}