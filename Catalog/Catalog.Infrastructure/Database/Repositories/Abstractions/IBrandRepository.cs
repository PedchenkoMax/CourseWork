using Catalog.Domain.Entities;

namespace Catalog.Infrastructure.Database.Repositories.Abstractions;

public interface IBrandRepository
{
    Task<List<BrandEntity>> GetAllAsync();
    Task<BrandEntity?> GetByIdAsync(Guid id);
    Task<bool> UpdateAsync(BrandEntity brand);
    Task<bool> RemoveByIdAsync(Guid id);
    Task<bool> AddAsync(BrandEntity brand);
    Task<bool> ExistsAsync(Guid id);
}