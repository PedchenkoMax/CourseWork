using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Cache.Services;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Catalog.Infrastructure.Cache.Repositories;

public class CachedProductRepository : IProductRepository
{
    private readonly ILogger<CachedProductRepository> logger; // TODO: cover methods with logs
    private readonly IProductRepository repository;
    private readonly RedisCacheManager cacheManager;
    private readonly IDatabase database;
    private readonly TimeSpan expiry;

    public CachedProductRepository(ILogger<CachedProductRepository> logger, IProductRepository repository,
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

    public async Task<List<ProductEntity>> GetAllAsync()
    {
        return await cacheManager.GetFromCacheAsync(database,
            key: "Product_All",
            fetchFromDb: () => repository.GetAllAsync(),
            expiry: expiry);
    }

    public async Task<ProductEntity?> GetByIdAsync(Guid id)
    {
        return await cacheManager.GetFromCacheAsync(database,
            key: $"Product_{id}",
            fetchFromDb: () => repository.GetByIdAsync(id),
            expiry: expiry);
    }

    public async Task<bool> UpdateAsync(ProductEntity product)
    {
        var result = await repository.UpdateAsync(product);

        await cacheManager.InvalidateCacheAsync(database,
            $"Product_{product.Id}",
            "Product_All");

        return result;
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        var result = await repository.RemoveByIdAsync(id);

        await cacheManager.InvalidateCacheAsync(database,
            $"Product_{id}",
            "Product_All");

        return result;
    }

    public async Task<bool> AddAsync(ProductEntity product)
    {
        var result = await repository.AddAsync(product);

        await cacheManager.InvalidateCacheAsync(database,
            $"Product_{product.Id}",
            "Product_All");

        return result;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var existsInCache = await database.KeyExistsAsync(key: $"Product_{id}");

        if (existsInCache)
            return true;

        return await repository.ExistsAsync(id);
    }
}