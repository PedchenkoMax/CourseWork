using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Exceptions;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.Infrastructure.Database.Schemas;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Database.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly ILogger<BrandRepository> logger;
    private readonly IDbConnection connection;

    public BrandRepository(ILogger<BrandRepository> logger, DapperDbContext context)
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

    public async Task<List<BrandEntity>> GetAllAsync()
    {
        var sql =
            $"""
            SELECT *
            FROM {BrandSchema.Table}
            """;

        logger.LogInformation("Fetching all brands");
        try
        {
            var res = await connection.QueryAsync<BrandEntity>(sql);
            logger.LogInformation("Successfully fetched all brands");
            return res.ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching all brands");
            throw new DatabaseException(e);
        }
    }

    public async Task<BrandEntity?> GetByIdAsync(Guid id)
    {
        var sql =
            $"""
            SELECT *
            FROM {BrandSchema.Table} 
            WHERE {BrandSchema.Columns.Id} = @{nameof(id)}
            """;

        logger.LogInformation("Fetching brand by ID {Id}", id);
        try
        {
            var res = await connection.QuerySingleOrDefaultAsync<BrandEntity>(sql, new { id });
            logger.LogInformation("Brand fetched successfully with ID {Id}", id);
            return res;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching brand by ID {Id}", id);
            throw new DatabaseException(e);
        }
    }

    public async Task<bool> UpdateAsync(BrandEntity brand)
    {
        var sql =
            $"""
            UPDATE {BrandSchema.Table} SET
                {BrandSchema.Columns.Name} = @{nameof(brand.Name)},
                {BrandSchema.Columns.Description} = @{nameof(brand.Description)},
                {BrandSchema.Columns.ImageFileName} = @{nameof(brand.ImageFileName)}
            WHERE {BrandSchema.Columns.Id} = @{nameof(brand.Id)}
            """;

        logger.LogInformation("Updating brand with ID {Id}", brand.Id);
        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, brand);
            if (rowsAffected > 0)
            {
                logger.LogInformation("Brand updated successfully with ID {Id}", brand.Id);
                return true;
            }
            else
            {
                logger.LogInformation("No brand found with ID {Id} to update", brand.Id);
                return false;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while updating brand with ID {Id}", brand.Id);
            throw new DatabaseException(e);
        }
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        var sql =
            $"""
            DELETE FROM {BrandSchema.Table} 
            WHERE {BrandSchema.Columns.Id} = @{nameof(id)}
            """;

        logger.LogInformation("Removing brand by ID {Id}", id);
        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, new { id });
            if (rowsAffected > 0)
            {
                logger.LogInformation("Brand removed successfully with ID {Id}", id);
                return true;
            }
            else
            {
                logger.LogInformation("No brand found with ID {Id} to remove", id);
                return false;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while removing brand by ID {Id}", id);
            throw new DatabaseException(e);
        }
    }

    public async Task<bool> AddAsync(BrandEntity brand)
    {
        var sql =
            $"""
            INSERT INTO {BrandSchema.Table} 
                ({BrandSchema.Columns.Id}, 
                 {BrandSchema.Columns.Name},
                 {BrandSchema.Columns.Description},
                 {BrandSchema.Columns.ImageFileName})
            VALUES 
                (@{nameof(brand.Id)},
                 @{nameof(brand.Name)},
                 @{nameof(brand.Description)},
                 @{nameof(brand.ImageFileName)})
            """;

        logger.LogInformation("Adding new brand with ID {Id}", brand.Id);
        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, brand);
            if (rowsAffected > 0)
            {
                logger.LogInformation("Brand added successfully with ID {Id}", brand.Id);
                return true;
            }
            else
            {
                logger.LogInformation("Brand could not be added with ID {Id}", brand.Id);
                return false;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while adding new brand with ID {Id}", brand.Id);
            throw new DatabaseException(e);
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var sql =
            $"""
            SELECT EXISTS (SELECT 1 
                           FROM {BrandSchema.Table}
                           WHERE {BrandSchema.Columns.Id} = @{nameof(id)})
            """;

        logger.LogInformation("Checking if brand exists with ID {Id}", id);
        try
        {
            var exists = await connection.ExecuteScalarAsync<bool>(sql, new { id });
            if (exists)
            {
                logger.LogInformation("Brand exists with ID {Id}", id);
                return exists;
            }
            else
            {
                logger.LogInformation("Brand does not exist with ID {Id}", id);
                return exists;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while checking if brand exists with ID {Id}", id);
            throw new DatabaseException(e);
        }
    }
}