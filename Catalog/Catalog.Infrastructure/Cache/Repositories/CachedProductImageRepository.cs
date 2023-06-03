using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Cache.Services;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Catalog.Infrastructure.Cache.Repositories;

public class CachedProductImageRepository : IProductImageRepository
{
    private readonly ILogger<CachedProductImageRepository> logger; // TODO: cover methods with logs
    private readonly IProductImageRepository repository;
    private readonly RedisCacheManager cacheManager;
    private readonly IDatabase database;
    private readonly TimeSpan expiry;

    public CachedProductImageRepository(ILogger<CachedProductImageRepository> logger, IProductImageRepository repository,
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

    public async Task<List<ProductImageEntity>> GetAllByProductIdAsync(Guid id)
    {
        return await cacheManager.GetFromCacheAsync(database,
            key: $"ProductImage_All_{id}",
            fetchFromDb: () => repository.GetAllByProductIdAsync(id),
            expiry: expiry);
    }

    public async Task<ProductImageEntity?> GetByIdAsync(Guid id)
    {
        return await cacheManager.GetFromCacheAsync(database,
            key: $"ProductImage_{id}",
            fetchFromDb: () => repository.GetByIdAsync(id),
            expiry: expiry);
    }

    public async Task<bool> UpdateAsync(ProductImageEntity productImage)
    {
        var result = await repository.UpdateAsync(productImage);

        await cacheManager.InvalidateCacheAsync(database,
            $"ProductImage_{productImage.Id}",
            $"ProductImage_All_{productImage.ProductId}");

        return result;
    }

    public async Task<bool> BatchUpdateAsync(List<ProductImageEntity> productImages)
    {
        var result = await repository.BatchUpdateAsync(productImages);

        foreach (var productImage in productImages)
        {
            await cacheManager.InvalidateCacheAsync(database,
                $"ProductImage_{productImage.Id}",
                $"ProductImage_All_{productImage.ProductId}");
        }

        return result;
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        var productImage = await GetByIdAsync(id);
        var result = await repository.RemoveByIdAsync(id);

        if (productImage != null)
        {
            await cacheManager.InvalidateCacheAsync(database,
                $"ProductImage_{id}",
                $"ProductImage_All_{productImage.ProductId}");
        }

        return result;
    }

    public async Task<bool> AddAsync(ProductImageEntity productImage)
    {
        var result = await repository.AddAsync(productImage);

        await cacheManager.InvalidateCacheAsync(database,
            $"ProductImage_{productImage.Id}",
            $"ProductImage_All_{productImage.ProductId}");

        return result;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var existsInCache = await database.KeyExistsAsync(key: $"ProductImage_{id}");

        if (existsInCache)
            return true;

        return await repository.ExistsAsync(id);
    }

    public async Task<int> GetProductImageCount(Guid productId)
    {
        var images = await GetAllByProductIdAsync(productId);

        if (images != null)
            return images.Count;

        return await repository.GetProductImageCount(productId);
    }
}