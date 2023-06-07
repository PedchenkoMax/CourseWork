using System.Data;
using Catalog.Domain.Entities;

namespace Catalog.Infrastructure.Database.Repositories.Abstractions;

public interface IProductRepository
{
    IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    Task<(List<ProductEntity>, int)> SearchByParametersAsync(int pageNumber, int pageSize, string orderBy, bool isAscending);
    Task<List<ProductEntity>> GetAllAsync();
    Task<ProductEntity?> GetByIdAsync(Guid id);
    Task<bool> UpdateAsync(ProductEntity product);
    Task<bool> RemoveByIdAsync(Guid id);
    Task<bool> AddAsync(ProductEntity product);
    Task<bool> ExistsAsync(Guid id);
}