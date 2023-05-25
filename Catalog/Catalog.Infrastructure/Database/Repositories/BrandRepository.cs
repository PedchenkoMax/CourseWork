using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.Infrastructure.Database.Schemas;
using Dapper;

namespace Catalog.Infrastructure.Database.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly IDbConnection connection;

    public BrandRepository(DbContext context)
    {
        connection = context.Connection;
    }

    public async Task<List<BrandEntity>> GetAllAsync()
    {
        var sql =
            $"""
            SELECT *
            FROM {BrandSchema.Table}
            """;

        var res = await connection.QueryAsync<BrandEntity>(sql);

        return res.ToList();
    }

    public async Task<BrandEntity?> GetByIdAsync(Guid id)
    {
        var sql =
            $"""
            SELECT *
            FROM {BrandSchema.Table} 
            WHERE {BrandSchema.Columns.Id} = @{nameof(id)}
            """;

        var res = await connection.QuerySingleOrDefaultAsync<BrandEntity>(sql, new { id });

        return res;
    }

    public async Task<bool> UpdateAsync(BrandEntity brand)
    {
        var sql =
            $"""
            UPDATE {BrandSchema.Table} SET
                {BrandSchema.Columns.Name} = @{nameof(brand.Name)},
                {BrandSchema.Columns.Description} = @{nameof(brand.Description)},
                {BrandSchema.Columns.ImageUrl} = @{nameof(brand.ImageUrl)},
                {BrandSchema.Columns.DisplayOrder} = @{nameof(brand.DisplayOrder)}
            WHERE {BrandSchema.Columns.Id} = @{nameof(brand.Id)}
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, brand);

        return rowsAffected > 0;
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        var sql =
            $"""
            DELETE FROM {BrandSchema.Table} 
            WHERE {BrandSchema.Columns.Id} = @{nameof(id)}
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, new { id });

        return rowsAffected > 0;
    }

    public async Task<bool> AddAsync(BrandEntity brand)
    {
        var sql =
            $"""
            INSERT INTO {BrandSchema.Table} 
                ({BrandSchema.Columns.Id}, 
                 {BrandSchema.Columns.Name},
                 {BrandSchema.Columns.Description},
                 {BrandSchema.Columns.ImageUrl},
                 {BrandSchema.Columns.DisplayOrder})
            VALUES 
                (@{nameof(brand.Id)},
                 @{nameof(brand.Name)},
                 @{nameof(brand.Description)},
                 @{nameof(brand.ImageUrl)},
                 @{nameof(brand.DisplayOrder)})
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, brand);

        return rowsAffected > 0;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var sql =
            $"""
            SELECT EXISTS (SELECT 1 
                           FROM {BrandSchema.Table}
                           WHERE {BrandSchema.Columns.Id} = @{nameof(id)})
            """;

        var exists = await connection.ExecuteScalarAsync<bool>(sql, new { id });

        return exists;
    }
}