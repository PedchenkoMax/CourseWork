using System.Data;
using Catalog.Domain.Entities;

namespace Catalog.Infrastructure.Database.Repositories.Abstractions;

public interface ICategoryRepository
{
    IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    Task<List<CategoryEntity>> GetAllAsync();
    Task<List<CategoryEntity>> GetSubcategoriesByParentCategoryIdAsync(Guid parentCategoryId);
    Task<CategoryEntity?> GetByIdAsync(Guid id);
    Task<bool> UpdateAsync(CategoryEntity category);
    Task<bool> RemoveByIdAsync(Guid id);
    Task<bool> AddAsync(CategoryEntity category);
    Task<bool> ExistsAsync(Guid id);
}