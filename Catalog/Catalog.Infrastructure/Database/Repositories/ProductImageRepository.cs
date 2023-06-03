using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Exceptions;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.Infrastructure.Database.Schemas;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Database.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly ILogger<ProductImageRepository> logger;
    private readonly IDbConnection connection;

    public ProductImageRepository(ILogger<ProductImageRepository> logger, DapperDbContext context)
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

    public async Task<List<ProductImageEntity>> GetAllByProductIdAsync(Guid id)
    {
        var sql =
            $"""
            SELECT *
            FROM {ProductImageSchema.Table} 
            WHERE {ProductImageSchema.Columns.ProductId} = @{nameof(id)}
            """;

        logger.LogInformation("Fetching all product images by product ID {Id}", id);
        try
        {
            var res = await connection.QueryAsync<ProductImageEntity>(sql, new { id });
            logger.LogInformation("Successfully fetched all product images by product ID {Id}", id);
            return res.ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching all product images by product ID {Id}", id);
            throw new DatabaseException(e);
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

        logger.LogInformation("Fetching product image by ID {Id}", id);
        try
        {
            var res = await connection.QuerySingleOrDefaultAsync<ProductImageEntity>(sql, new { id });
            logger.LogInformation("Product image fetched successfully with ID {Id}", id);
            return res;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching product image by ID {Id}", id);
            throw new DatabaseException(e);
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

        logger.LogInformation("Updating product image with ID {Id}", productImage.Id);
        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, productImage);
            if (rowsAffected > 0)
            {
                logger.LogInformation("Product image updated successfully with ID {Id}", productImage.Id);
                return true;
            }
            else
            {
                logger.LogInformation("No product image found with ID {Id} to update", productImage.Id);
                return false;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while updating product image with ID {Id}", productImage.Id);
            throw new DatabaseException(e);
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

        logger.LogInformation("Batch updating product images");
        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, productImages);
            if (rowsAffected == productImages.Count)
            {
                logger.LogInformation("Product images batch updated successfully");
                return true;
            }
            else
            {
                logger.LogInformation("Product images batch update failed");
                return false;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while batch updating product images");
            throw new DatabaseException(e);
        }
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        var sql =
            $"""
            DELETE FROM {ProductImageSchema.Table} 
            WHERE {ProductImageSchema.Columns.Id} = @{nameof(id)}
            """;

        logger.LogInformation("Removing product image by ID {Id}", id);
        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, new { id });
            if (rowsAffected > 0)
            {
                logger.LogInformation("Product image removed successfully with ID {Id}", id);
                return true;
            }
            else
            {
                logger.LogInformation("No product image found with ID {Id} to remove", id);
                return false;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while removing product image by ID {Id}", id);
            throw new DatabaseException(e);
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

        logger.LogInformation("Adding new product image with ID {Id}", productImage.Id);
        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, productImage);
            if (rowsAffected > 0)
            {
                logger.LogInformation("Product image added successfully with ID {Id}", productImage.Id);
                return true;
            }
            else
            {
                logger.LogInformation("Product image could not be added with ID {Id}", productImage.Id);
                return false;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while adding new product image with ID {Id}", productImage.Id);
            throw new DatabaseException(e);
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

        logger.LogInformation("Checking if product image exists with ID {Id}", id);
        try
        {
            var exists = await connection.ExecuteScalarAsync<bool>(sql, new { id });
            if (exists)
            {
                logger.LogInformation("Product image exists with ID {Id}", id);
                return exists;
            }
            else
            {
                logger.LogInformation("Product image does not exist with ID {Id}", id);
                return exists;
            }
        }
        catch (Exception e)
        {
            throw new DatabaseException(e);
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

        logger.LogInformation("Getting product image count for product ID {Id}", productId);
        try
        {
            var count = await connection.ExecuteScalarAsync<int>(sql, new { productId });
            logger.LogInformation("Product image count retrieved successfully for product ID {Id}: {Count}", productId, count);
            return count;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while getting product image count for product ID {Id}", productId);
            throw new DatabaseException(e);
        }
    }
}