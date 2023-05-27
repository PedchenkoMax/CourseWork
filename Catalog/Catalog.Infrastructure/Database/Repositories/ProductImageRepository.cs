using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.Infrastructure.Database.Schemas;
using Dapper;

namespace Catalog.Infrastructure.Database.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly IDbConnection connection;

    public ProductImageRepository(DbContext context)
    {
        connection = context.Connection;
    }

    public async Task<List<ProductImageEntity>> GetAllByProductIdAsync(Guid id)
    {
        var sql =
            $"""
            SELECT *
            FROM {ProductImageSchema.Table} 
            WHERE {ProductImageSchema.Columns.ProductId} = @{nameof(id)}
            """;

        var res = await connection.QueryAsync<ProductImageEntity>(sql, new { id });

        return res.ToList();
    }

    public async Task<ProductImageEntity?> GetByIdAsync(Guid id)
    {
        var sql =
            $"""
            SELECT *
            FROM {ProductImageSchema.Table} 
            WHERE {ProductImageSchema.Columns.Id} = @{nameof(id)}
            """;

        var res = await connection.QuerySingleOrDefaultAsync<ProductImageEntity>(sql, new { id });

        return res;
    }

    public async Task<bool> UpdateAsync(ProductImageEntity productImage)
    {
        var sql =
            $"""
            UPDATE {ProductImageSchema.Table} SET
                {ProductImageSchema.Columns.ProductId} = @{nameof(productImage.ProductId)},
                {ProductImageSchema.Columns.ImageUrl} = @{nameof(productImage.ImageUrl)},
                {ProductImageSchema.Columns.DisplayOrder} = @{nameof(productImage.DisplayOrder)}
            WHERE {ProductImageSchema.Columns.Id} = @{nameof(productImage.Id)}
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, productImage);

        return rowsAffected > 0;
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        var sql =
            $"""
            DELETE FROM {ProductImageSchema.Table} 
            WHERE {ProductImageSchema.Columns.Id} = @{nameof(id)}
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, new { id });

        return rowsAffected > 0;
    }

    public async Task<bool> AddAsync(ProductImageEntity productImage)
    {
        var sql =
            $"""
            INSERT INTO {ProductImageSchema.Table} 
                ({ProductImageSchema.Columns.Id}, 
                 {ProductImageSchema.Columns.ProductId},
                 {ProductImageSchema.Columns.ImageUrl},
                 {ProductImageSchema.Columns.DisplayOrder})
            VALUES 
                (@{nameof(productImage.Id)},
                 @{nameof(productImage.ProductId)},
                 @{nameof(productImage.ImageUrl)},
                 @{nameof(productImage.DisplayOrder)})
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, productImage);

        return rowsAffected > 0;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var sql =
            $"""
            SELECT EXISTS (SELECT 1 
                           FROM {ProductImageSchema.Table}
                           WHERE {ProductImageSchema.Columns.Id} = @{nameof(id)})
            """;

        var exists = await connection.ExecuteScalarAsync<bool>(sql, new { id });

        return exists;
    }

    public async Task<int> GetProductImageCount(Guid productId)
    {
        var sql =
            $"""
            SELECT COUNT(*)
            FROM {ProductImageSchema.Table}
            WHERE {ProductImageSchema.Columns.ProductId} = @{nameof(productId)}
            """;

        var count = await connection.ExecuteScalarAsync<int>(sql, new { productId });

        return count;
    }
}