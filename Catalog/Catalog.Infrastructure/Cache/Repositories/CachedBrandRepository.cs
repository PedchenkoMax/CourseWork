using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Cache.Services;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using StackExchange.Redis;

namespace Catalog.Infrastructure.Cache.Repositories;

public class CachedBrandRepository : IBrandRepository
{
    private readonly IBrandRepository repository;
    private readonly RedisCacheManager cacheManager;
    private readonly IDatabase database;
    private readonly TimeSpan expiry;

    public CachedBrandRepository(IBrandRepository repository, IConnectionMultiplexer connectionMultiplexer,
        RedisCacheManager cacheManager)
    {
        this.repository = repository;
        this.cacheManager = cacheManager;
        database = connectionMultiplexer.GetDatabase();
        expiry = TimeSpan.FromMinutes(5);
    }

    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        return repository.BeginTransaction(isolationLevel);
    }

    public async Task<List<BrandEntity>> GetAllAsync()
    {
        return await cacheManager.GetFromCacheAsync(database,
            key: "Brand_All",
            fetchFromDb: () => repository.GetAllAsync(),
            expiry: expiry);
    }

    public async Task<BrandEntity?> GetByIdAsync(Guid id)
    {
        return await cacheManager.GetFromCacheAsync(database,
            key: $"Brand_{id}",
            fetchFromDb: () => repository.GetByIdAsync(id),
            expiry: expiry);
    }

    public async Task<bool> UpdateAsync(BrandEntity brand)
    {
        var result = await repository.UpdateAsync(brand);

        await cacheManager.InvalidateCacheAsync(database,
            $"Brand_{brand.Id}",
            "Brand_All");

        return result;
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        var result = await repository.RemoveByIdAsync(id);

        await cacheManager.InvalidateCacheAsync(database,
            $"Brand_{id}",
            "Brand_All");

        return result;
    }

    public async Task<bool> AddAsync(BrandEntity brand)
    {
        var result = await repository.AddAsync(brand);

        await cacheManager.InvalidateCacheAsync(database,
            $"Brand_{brand.Id}",
            "Brand_All");

        return result;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var existsInCache = await database.KeyExistsAsync(key: $"Brand_{id}");

        if (existsInCache)
            return true;

        return await repository.ExistsAsync(id);
    }
}