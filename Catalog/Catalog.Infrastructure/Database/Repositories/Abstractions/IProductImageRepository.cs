using System.Data;
using Catalog.Domain.Entities;

namespace Catalog.Infrastructure.Database.Repositories.Abstractions;

public interface IProductImageRepository
{
    IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    Task<List<ProductImageEntity>> GetAllByProductIdAsync(Guid id);
    Task<ProductImageEntity?> GetByIdAsync(Guid id);
    Task<bool> UpdateAsync(ProductImageEntity productImage);
    Task<bool> BatchUpdateAsync(List<ProductImageEntity> productImages);
    Task<bool> RemoveByIdAsync(Guid id);
    Task<bool> AddAsync(ProductImageEntity productImage);
    Task<bool> ExistsAsync(Guid id);
    Task<int> GetProductImageCount(Guid productId);
}