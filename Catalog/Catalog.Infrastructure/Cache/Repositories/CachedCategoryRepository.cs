using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Cache.Extensions;
using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using StackExchange.Redis;

namespace Catalog.Infrastructure.Cache.Repositories;

public class CachedCategoryRepository : ICategoryRepository
{
    private readonly ICategoryRepository repository;
    private readonly IDatabase database;
    private readonly TimeSpan expiry;

    public CachedCategoryRepository(ICategoryRepository repository,  IConnectionMultiplexer connectionMultiplexer)
    {
        this.repository = repository;
        database = connectionMultiplexer.GetDatabase();
        expiry = TimeSpan.FromMinutes(5);
    }

    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        return repository.BeginTransaction(isolationLevel);

    }

    public async Task<List<CategoryEntity>> GetAllAsync()
    {
        return await database.GetFromCacheAsync(
            key: "Category_All",
            fetchFromDb: () => repository.GetAllAsync(),
            expiry: expiry);
    }

    public async Task<List<CategoryEntity>> GetSubcategoriesByParentCategoryIdAsync(Guid parentCategoryId)
    {
        return await database.GetFromCacheAsync(
            key: $"Category_Sub_{parentCategoryId}",
            fetchFromDb: () => repository.GetSubcategoriesByParentCategoryIdAsync(parentCategoryId),
            expiry: expiry);
    }

    public async Task<CategoryEntity?> GetByIdAsync(Guid id)
    {
        return await database.GetFromCacheAsync(
            key: $"Category_{id}",
            fetchFromDb: () => repository.GetByIdAsync(id),
            expiry: expiry);
    }

    public async Task<bool> UpdateAsync(CategoryEntity category)
    {
        var result = await repository.UpdateAsync(category);

        await database.InvalidateCacheAsync($"Category_{category.Id}", $"Category_Sub_{category.ParentCategoryId}", "Category_All");

        return result;
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        var result = await repository.RemoveByIdAsync(id);

        var category = await GetByIdAsync(id);
        if (category != null)
        {
            await database.InvalidateCacheAsync($"Category_{id}", $"Category_Sub_{category.ParentCategoryId}", "Category_All");
        }

        return result;
    }

    public async Task<bool> AddAsync(CategoryEntity category)
    {
        var result = await repository.AddAsync(category);

        await database.InvalidateCacheAsync($"Category_{category.Id}", $"Category_Sub_{category.ParentCategoryId}", "Category_All");

        return result;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var existsInCache = await database.KeyExistsAsync(key: $"Category_{id}");

        if (existsInCache)
            return true;

        return await repository.ExistsAsync(id);
    }
}