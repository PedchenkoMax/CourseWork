using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Cache.Extensions;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using StackExchange.Redis;

namespace Catalog.Infrastructure.Cache.Repositories;

public class CachedProductRepository : IProductRepository
{
    private readonly IProductRepository repository;
    private readonly IDatabase database;
    private readonly TimeSpan expiry;

    public CachedProductRepository(IProductRepository repository, IConnectionMultiplexer connectionMultiplexer)
    {
        this.repository = repository;
        database = connectionMultiplexer.GetDatabase();
        expiry = TimeSpan.FromMinutes(5);
    }

    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        return repository.BeginTransaction(isolationLevel);
    }

    public async Task<List<ProductEntity>> GetAllAsync()
    {
        return await database.GetFromCacheAsync(
            key: "Product_All",
            fetchFromDb: () => repository.GetAllAsync(),
            expiry: expiry);
    }

    public async Task<ProductEntity?> GetByIdAsync(Guid id)
    {
        return await database.GetFromCacheAsync(
            key: $"Product_{id}",
            fetchFromDb: () => repository.GetByIdAsync(id),
            expiry: expiry);
    }

    public async Task<bool> UpdateAsync(ProductEntity product)
    {
        var result = await repository.UpdateAsync(product);

        await database.InvalidateCacheAsync($"Product_{product.Id}", "Product_All");

        return result;
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        var result = await repository.RemoveByIdAsync(id);

        await database.InvalidateCacheAsync($"Product_{id}", "Product_All");

        return result;
    }

    public async Task<bool> AddAsync(ProductEntity product)
    {
        var result = await repository.AddAsync(product);

        await database.InvalidateCacheAsync($"Product_{product.Id}", "Product_All");

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