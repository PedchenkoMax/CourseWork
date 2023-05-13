using Catalog.Domain.Entities;

namespace Catalog.Infrastructure.Database.Repositories.Abstractions;

public interface ICategoryRepository
{
    Task<List<CategoryEntity>> GetAllAsync();
    Task<CategoryEntity?> GetByIdAsync(Guid id);
    Task<bool> UpdateAsync(CategoryEntity category);
    Task<bool> RemoveByIdAsync(Guid id);
    Task<bool> AddAsync(CategoryEntity category);
}