using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.Infrastructure.Database.Schemas;
using Dapper;

namespace Catalog.Infrastructure.Database.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly IDbConnection connection;

    public ProductImageRepository(DapperDbContext context)
    {
        connection = context.Connection;
    }

    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        connection.Open();
        return connection.BeginTransaction(isolationLevel);
    }

    public async Task<List<ProductImageEntity>> GetAllByProductIdAsync(Guid id)
    {
        var sql =
            $"""
            SELECT *
            FROM {ProductImageSchema.Table} 
            WHERE {ProductImageSchema.Columns.ProductId} = @{nameof(id)}
            """;

        try
        {
            var res = await connection.QueryAsync<ProductImageEntity>(sql, new { id });

            return res.ToList();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<ProductImageEntity?> GetByIdAsync(Guid id)
    {
        var sql =
            $"""
            SELECT *
            FROM {ProductImageSchema.Table} 
            WHERE {ProductImageSchema.Columns.Id} = @{nameof(id)}
            """;

        try
        {
            var res = await connection.QuerySingleOrDefaultAsync<ProductImageEntity>(sql, new { id });

            return res;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<bool> UpdateAsync(ProductImageEntity productImage)
    {
        var sql =
            $"""
            UPDATE {ProductImageSchema.Table} SET
                {ProductImageSchema.Columns.ProductId} = @{nameof(productImage.ProductId)},
                {ProductImageSchema.Columns.ImageFileName} = @{nameof(productImage.ImageFileName)},
                {ProductImageSchema.Columns.DisplayOrder} = @{nameof(productImage.DisplayOrder)}
            WHERE {ProductImageSchema.Columns.Id} = @{nameof(productImage.Id)}
            """;

        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, productImage);

            return rowsAffected > 0;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<bool> BatchUpdateAsync(List<ProductImageEntity> productImages)
    {
        var sql =
            $"""
            UPDATE {ProductImageSchema.Table} SET
                {ProductImageSchema.Columns.ProductId} = @{nameof(ProductImageEntity.ProductId)},
                {ProductImageSchema.Columns.ImageFileName} = @{nameof(ProductImageEntity.ImageFileName)},
                {ProductImageSchema.Columns.DisplayOrder} = @{nameof(ProductImageEntity.DisplayOrder)}
            WHERE {ProductImageSchema.Columns.Id} = @{nameof(ProductImageEntity.Id)}
            """;

        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, productImages);

            return rowsAffected == productImages.Count;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        var sql =
            $"""
            DELETE FROM {ProductImageSchema.Table} 
            WHERE {ProductImageSchema.Columns.Id} = @{nameof(id)}
            """;

        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, new { id });

            return rowsAffected > 0;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<bool> AddAsync(ProductImageEntity productImage)
    {
        var sql =
            $"""
            INSERT INTO {ProductImageSchema.Table} 
                ({ProductImageSchema.Columns.Id}, 
                 {ProductImageSchema.Columns.ProductId},
                 {ProductImageSchema.Columns.ImageFileName},
                 {ProductImageSchema.Columns.DisplayOrder})
            VALUES 
                (@{nameof(productImage.Id)},
                 @{nameof(productImage.ProductId)},
                 @{nameof(productImage.ImageFileName)},
                 @{nameof(productImage.DisplayOrder)})
            """;

        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, productImage);

            return rowsAffected > 0;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var sql =
            $"""
            SELECT EXISTS (SELECT 1 
                           FROM {ProductImageSchema.Table}
                           WHERE {ProductImageSchema.Columns.Id} = @{nameof(id)})
            """;

        try
        {
            var exists = await connection.ExecuteScalarAsync<bool>(sql, new { id });

            return exists;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<int> GetProductImageCount(Guid productId)
    {
        var sql =
            $"""
            SELECT COUNT(*)
            FROM {ProductImageSchema.Table}
            WHERE {ProductImageSchema.Columns.ProductId} = @{nameof(productId)}
            """;

        try
        {
            var count = await connection.ExecuteScalarAsync<int>(sql, new { productId });

            return count;
        }
        catch (Exception e)
        {
            throw;
        }
    }
}