using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.Infrastructure.Database.Schemas;
using Dapper;

namespace Catalog.Infrastructure.Database.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IDbConnection connection;

    public CategoryRepository(DapperDbContext context)
    {
        connection = context.Connection;
    }

    public async Task<List<CategoryEntity>> GetAllAsync()
    {
        var sql =
            $"""
            SELECT *
            FROM {CategorySchema.Table}
            """;

        try
        {
            var res = await connection.QueryAsync<CategoryEntity>(sql);

            return res.ToList();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<List<CategoryEntity>> GetSubcategoriesByParentCategoryIdAsync(Guid parentCategoryId)
    {
        var sql =
            $"""
            SELECT *
            FROM {CategorySchema.Table} 
            WHERE {CategorySchema.Columns.ParentCategoryId} = @{nameof(parentCategoryId)}
            """;

        try
        {
            var res = await connection.QueryAsync<CategoryEntity>(sql, new { parentCategoryId });

            return res.ToList();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<CategoryEntity?> GetByIdAsync(Guid id)
    {
        var sql =
            $"""
            SELECT *
            FROM {CategorySchema.Table} 
            WHERE {CategorySchema.Columns.Id} = @{nameof(id)}
            """;

        try
        {
            var res = await connection.QuerySingleOrDefaultAsync<CategoryEntity>(sql, new { id });

            return res;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<bool> UpdateAsync(CategoryEntity category)
    {
        var sql =
            $"""
            UPDATE {CategorySchema.Table} SET
                {CategorySchema.Columns.ParentCategoryId} = @{nameof(category.ParentCategoryId)},
                {CategorySchema.Columns.Name} = @{nameof(category.Name)},
                {CategorySchema.Columns.Description} = @{nameof(category.Description)},
                {CategorySchema.Columns.ImageFileName} = @{nameof(category.ImageFileName)}
            WHERE {CategorySchema.Columns.Id} = @{nameof(category.Id)}
            """;

        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, category);

            return rowsAffected > 0;
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
            DELETE FROM {CategorySchema.Table} 
            WHERE {CategorySchema.Columns.Id} = @{nameof(id)}
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

    public async Task<bool> AddAsync(CategoryEntity category)
    {
        var sql =
            $"""
            INSERT INTO {CategorySchema.Table} 
                ({CategorySchema.Columns.Id}, 
                 {CategorySchema.Columns.ParentCategoryId},
                 {CategorySchema.Columns.Name},
                 {CategorySchema.Columns.Description},
                 {CategorySchema.Columns.ImageFileName})
            VALUES 
                (@{nameof(category.Id)},
                 @{nameof(category.ParentCategoryId)},
                 @{nameof(category.Name)},
                 @{nameof(category.Description)},
                 @{nameof(category.ImageFileName)})
            """;

        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, category);

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
                           FROM {CategorySchema.Table}
                           WHERE {CategorySchema.Columns.Id} = @{nameof(id)})
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
}