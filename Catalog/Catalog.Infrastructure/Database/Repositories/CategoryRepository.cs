using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Exceptions;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.Infrastructure.Database.Schemas;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Database.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ILogger<CategoryRepository> logger;
    private readonly IDbConnection connection;

    public CategoryRepository(ILogger<CategoryRepository> logger, DapperDbContext context)
    {
        this.logger = logger;
        connection = context.Connection;
    }

    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        logger.LogInformation("Database connection opened");
        connection.Open();

        logger.LogInformation("Starting a new transaction with isolation level {IsolationLevel}", isolationLevel);
        return connection.BeginTransaction(isolationLevel);
    }

    public async Task<List<CategoryEntity>> GetAllAsync()
    {
        var sql =
            $"""
            SELECT *
            FROM {CategorySchema.Table}
            """;

        logger.LogInformation("Fetching all categories");
        try
        {
            var res = await connection.QueryAsync<CategoryEntity>(sql);
            logger.LogInformation("Successfully fetched all categories");
            return res.ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching all categories");
            throw new DatabaseException(e);
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

        logger.LogInformation("Fetching subcategories by parent category ID {Id}", parentCategoryId);
        try
        {
            var res = await connection.QueryAsync<CategoryEntity>(sql, new { parentCategoryId });
            logger.LogInformation("Successfully fetched subcategories for parent category ID {Id}", parentCategoryId);
            return res.ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching subcategories by parent category ID {Id}", parentCategoryId);
            throw new DatabaseException(e);
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

        logger.LogInformation("Fetching category by ID {Id}", id);
        try
        {
            var res = await connection.QuerySingleOrDefaultAsync<CategoryEntity>(sql, new { id });
            logger.LogInformation("Category fetched successfully with ID {Id}", id);
            return res;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching category by ID {Id}", id);
            throw new DatabaseException(e);
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

        logger.LogInformation("Updating category with ID {Id}", category.Id);
        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, category);
            if (rowsAffected > 0)
            {
                logger.LogInformation("Category updated successfully with ID {Id}", category.Id);
                return true;
            }
            else
            {
                logger.LogInformation("No category found with ID {Id} to update", category.Id);
                return false;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while updating category with ID {Id}", category.Id);
            throw new DatabaseException(e);
        }
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        var sql =
            $"""
            DELETE FROM {CategorySchema.Table} 
            WHERE {CategorySchema.Columns.Id} = @{nameof(id)}
            """;

        logger.LogInformation("Removing category by ID {Id}", id);
        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, new { id });
            if (rowsAffected > 0)
            {
                logger.LogInformation("Category removed successfully with ID {Id}", id);
                return true;
            }
            else
            {
                logger.LogInformation("No category found with ID {Id} to remove", id);
                return false;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while removing category by ID {Id}", id);
            throw new DatabaseException(e);
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

        logger.LogInformation("Adding new category with ID {Id}", category.Id);
        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, category);
            if (rowsAffected > 0)
            {
                logger.LogInformation("Category added successfully with ID {Id}", category.Id);
                return true;
            }
            else
            {
                logger.LogInformation("Category could not be added with ID {Id}", category.Id);
                return false;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while adding new category with ID {Id}", category.Id);
            throw new DatabaseException(e);
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

        logger.LogInformation("Checking if category exists with ID {Id}", id);
        try
        {
            var exists = await connection.ExecuteScalarAsync<bool>(sql, new { id });
            if (exists)
            {
                logger.LogInformation("Category exists with ID {Id}", id);
                return exists;
            }
            else
            {
                logger.LogInformation("Category does not exist with ID {Id}", id);
                return exists;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while checking if category exists with ID {Id}", id);
            throw new DatabaseException(e);
        }
    }
}