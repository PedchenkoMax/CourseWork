using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Cache.Services;
using Catalog.Infrastructure.Database;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Catalog.Infrastructure.Cache.Repositories;

public class CachedCategoryRepository : ICategoryRepository
{
    private readonly ILogger<CachedCategoryRepository> logger; // TODO: cover methods with logs
    private readonly ICategoryRepository repository;
    private readonly RedisCacheManager cacheManager;
    private readonly IDatabase database;
    private readonly TimeSpan expiry;

    public CachedCategoryRepository(ILogger<CachedCategoryRepository> logger, ICategoryRepository repository,
        IConnectionMultiplexer connectionMultiplexer, RedisCacheManager cacheManager)
    {
        this.logger = logger;
        this.repository = repository;
        this.cacheManager = cacheManager;
        database = connectionMultiplexer.GetDatabase();
        expiry = TimeSpan.FromMinutes(5);
    }

    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        return repository.BeginTransaction(isolationLevel);
    }

    public async Task<List<CategoryEntity>> GetAllAsync()
    {
        return await cacheManager.GetFromCacheAsync(database,
            key: "Category_All",
            fetchFromDb: () => repository.GetAllAsync(),
            expiry: expiry);
    }

    public async Task<List<CategoryEntity>> GetSubcategoriesByParentCategoryIdAsync(Guid parentCategoryId)
    {
        return await cacheManager.GetFromCacheAsync(database,
            key: $"Category_Sub_{parentCategoryId}",
            fetchFromDb: () => repository.GetSubcategoriesByParentCategoryIdAsync(parentCategoryId),
            expiry: expiry);
    }

    public async Task<CategoryEntity?> GetByIdAsync(Guid id)
    {
        return await cacheManager.GetFromCacheAsync(database,
            key: $"Category_{id}",
            fetchFromDb: () => repository.GetByIdAsync(id),
            expiry: expiry);
    }

    public async Task<bool> UpdateAsync(CategoryEntity category)
    {
        var result = await repository.UpdateAsync(category);

        await cacheManager.InvalidateCacheAsync(database,
            $"Category_{category.Id}",
            $"Category_Sub_{category.ParentCategoryId}",
            "Category_All");

        return result;
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        var result = await repository.RemoveByIdAsync(id);

        var category = await GetByIdAsync(id);
        if (category != null)
        {
            await cacheManager.InvalidateCacheAsync(database,
                $"Category_{id}",
                $"Category_Sub_{category.ParentCategoryId}",
                "Category_All");
        }

        return result;
    }

    public async Task<bool> AddAsync(CategoryEntity category)
    {
        var result = await repository.AddAsync(category);

        await cacheManager.InvalidateCacheAsync(database,
            $"Category_{category.Id}",
            $"Category_Sub_{category.ParentCategoryId}",
            "Category_All");

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