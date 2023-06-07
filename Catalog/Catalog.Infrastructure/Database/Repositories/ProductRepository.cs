using System.Data;
using System.Text;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.Infrastructure.Database.Schemas;
using Catalog.Infrastructure.Exceptions;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Database.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ILogger<ProductRepository> logger;
    private readonly IDbConnection connection;

    public ProductRepository(ILogger<ProductRepository> logger, DapperDbContext context)
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

    public async Task<(List<ProductEntity>, int)> SearchByParametersAsync(int pageNumber, int pageSize, string orderBy, bool isAscending)
    {
        var sql = new StringBuilder(
            $"""
            SELECT p.*, b.*, c.*, i.*
            FROM {ProductSchema.Table} AS p
            LEFT JOIN {BrandSchema.Table} AS b
                ON p.{ProductSchema.Columns.BrandId} = b.{BrandSchema.Columns.Id}
            LEFT JOIN {CategorySchema.Table} AS c
                ON p.{ProductSchema.Columns.CategoryId} = c.{CategorySchema.Columns.Id}
            LEFT JOIN {ProductImageSchema.Table} AS i
                ON p.{ProductSchema.Columns.Id} = i.{ProductImageSchema.Columns.ProductId}
            """);

        var sortOrder = isAscending ? "ASC" : "DESC";
        var offSet = (pageNumber - 1)  * pageSize;
        sql.AppendLine($" ORDER BY p.{orderBy} {sortOrder}");
        sql.AppendLine(" OFFSET @Offset ROWS");
        sql.AppendLine(" FETCH NEXT @PageSize ROWS ONLY");

        logger.LogInformation("Searching for products by parameters (PageNumber: {PageNumber}, PageSize: {PageSize}, OrderBy: {OrderBy}, IsAscending: {IsAscending})",
            pageNumber, pageSize, orderBy, isAscending);
        try
        {
            var productDictionary = new Dictionary<string, ProductEntity>();

            var productEntities = (await connection.QueryAsync<ProductEntity, BrandEntity, CategoryEntity, ProductImageEntity, ProductEntity>(
                sql.ToString(),
                (product, brand, category, image) =>
                {
                    var key = product.Id.ToString();
                    if (!productDictionary.TryGetValue(key, out var productEntry))
                    {
                        productEntry = product;
                        productDictionary.Add(key, productEntry);
                    }

                    productEntry.Brand = brand;
                    productEntry.Category = category;

                    if (image != null)
                        productEntry.Images.Add(image);

                    return productEntry;
                },
                new { Offset = offSet, PageSize = pageSize },
                splitOn: $"{BrandSchema.Columns.Id},{CategorySchema.Columns.Id},{ProductImageSchema.Columns.ProductId}"
            )).Distinct().ToList();
        
//             var countSql = new StringBuilder(
//                 $"""
//                 SELECT COUNT(*)
//                 FROM {ProductSchema.Table} AS p
//                 LEFT JOIN {BrandSchema.Table} AS b
//                     ON p.{ProductSchema.Columns.BrandId} = b.{BrandSchema.Columns.Id}
//                 LEFT JOIN {CategorySchema.Table} AS c
//                     ON p.{ProductSchema.Columns.CategoryId} = c.{CategorySchema.Columns.Id}
//                 LEFT JOIN {ProductImageSchema.Table} AS i
//                     ON p.{ProductSchema.Columns.Id} = i.{ProductImageSchema.Columns.ProductId}
//                 """);
            
             var countSql = new StringBuilder(
                 $"""
                 SELECT COUNT(*)
                 FROM {ProductSchema.Table}
                 """);

            var totalCount = await connection.ExecuteScalarAsync<int>(countSql.ToString());
        
            logger.LogInformation("Successfully fetched {ProductCount} products with specified parameters (PageNumber: {PageNumber}, PageSize: {PageSize}," +
                                  " OrderBy: {OrderBy}, IsAscending: {IsAscending})", productEntities.Count, pageNumber, pageSize, orderBy, isAscending);
            return (productEntities, totalCount);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while searching for products by parameters (PageNumber: {PageNumber}, PageSize: {PageSize}," +
                               " OrderBy: {OrderBy}, IsAscending: {IsAscending})", pageNumber, pageSize, orderBy, isAscending);

            throw new DatabaseException(e);
        }
    }

    public async Task<List<ProductEntity>> GetAllAsync()
    {
        var sql =
            $"""
            SELECT p.*, b.*, c.*, i.*
            FROM {ProductSchema.Table} AS p
            LEFT JOIN {BrandSchema.Table} AS b
                ON p.{ProductSchema.Columns.BrandId} = b.{BrandSchema.Columns.Id}
            LEFT JOIN {CategorySchema.Table} AS c
                ON p.{ProductSchema.Columns.CategoryId} = c.{CategorySchema.Columns.Id}
            LEFT JOIN {ProductImageSchema.Table} AS i
                ON p.{ProductSchema.Columns.Id} = i.{ProductImageSchema.Columns.ProductId}
            """;

        logger.LogInformation("Fetching all products");

        try
        {
            var productDictionary = new Dictionary<Guid, ProductEntity>();

            await connection.QueryAsync<ProductEntity, BrandEntity, CategoryEntity, ProductImageEntity, ProductEntity>(
                sql,
                (product, brand, category, image) =>
                {
                    if (!productDictionary.TryGetValue(product.Id, out var productEntry))
                    {
                        productEntry = product;
                        productDictionary.Add(productEntry.Id, productEntry);
                    }

                    productEntry.Brand = brand;
                    productEntry.Category = category;

                    if (image != null)
                        productEntry.Images.Add(image);

                    return productEntry;
                },
                splitOn: $"{BrandSchema.Columns.Id},{CategorySchema.Columns.Id},{ProductImageSchema.Columns.Id}");

            logger.LogInformation("Successfully fetched all products");
            return productDictionary.Values.ToList();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching all products");
            throw new DatabaseException(e);
        }
    }

    public async Task<ProductEntity?> GetByIdAsync(Guid id)
    {
        var sql =
            $"""
            SELECT p.*, b.*, c.*, i.*
            FROM {ProductSchema.Table} AS p
            LEFT JOIN {BrandSchema.Table} AS b
                ON p.{ProductSchema.Columns.BrandId} = b.{BrandSchema.Columns.Id}
            LEFT JOIN {CategorySchema.Table} AS c
                ON p.{ProductSchema.Columns.CategoryId} = c.{CategorySchema.Columns.Id}
            LEFT JOIN {ProductImageSchema.Table} AS i
                ON p.{ProductSchema.Columns.Id} = i.{ProductImageSchema.Columns.ProductId}
            WHERE p.{ProductSchema.Columns.Id} = @{nameof(id)}
            """;

        logger.LogInformation("Fetching product by ID {Id}", id);
        try
        {
            var productDictionary = new Dictionary<Guid, ProductEntity>();

            await connection.QueryAsync<ProductEntity, BrandEntity, CategoryEntity, ProductImageEntity, ProductEntity>(
                sql,
                (product, brand, category, image) =>
                {
                    if (!productDictionary.TryGetValue(product.Id, out var productEntry))
                    {
                        productEntry = product;
                        productDictionary.Add(productEntry.Id, productEntry);
                    }

                    productEntry.Brand = brand;
                    productEntry.Category = category;

                    if (image != null)
                        productEntry.Images.Add(image);

                    return productEntry;
                },
                new { Id = id },
                splitOn: $"{BrandSchema.Columns.Id},{CategorySchema.Columns.Id},{ProductImageSchema.Columns.Id}");

            logger.LogInformation("Product fetched successfully with ID {Id}", id);
            return productDictionary.Values.FirstOrDefault();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching product by ID {Id}", id);
            throw new DatabaseException(e);
        }
    }

    public async Task<bool> UpdateAsync(ProductEntity product)
    {
        var sql =
            $"""
            UPDATE {ProductSchema.Table} SET
                {ProductSchema.Columns.BrandId} = @{nameof(product.BrandId)},
                {ProductSchema.Columns.CategoryId} = @{nameof(product.CategoryId)},
                {ProductSchema.Columns.Name} = @{nameof(product.Name)},
                {ProductSchema.Columns.Slug} = @{nameof(product.Slug)},
                {ProductSchema.Columns.Description} = @{nameof(product.Description)},
                {ProductSchema.Columns.Price} = @{nameof(product.Price)},
                {ProductSchema.Columns.Discount} = @{nameof(product.Discount)},
                {ProductSchema.Columns.SKU} = @{nameof(product.SKU)},
                {ProductSchema.Columns.Stock} = @{nameof(product.Stock)},
                {ProductSchema.Columns.Availability} = @{nameof(product.Availability)}
            WHERE {ProductSchema.Columns.Id} = @{nameof(product.Id)}
            """;

        logger.LogInformation("Updating product with ID {Id}", product.Id);
        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, product);

            if (rowsAffected > 0)
            {
                logger.LogInformation("Product updated successfully with ID {Id}", product.Id);
                return true;
            }
            else
            {
                logger.LogInformation("No product found with ID {Id} to update", product.Id);
                return false;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while updating product with ID {Id}", product.Id);
            throw new DatabaseException(e);
        }
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        var sql =
            $"""
            DELETE FROM {ProductSchema.Table}
            WHERE {ProductSchema.Columns.Id} = @{nameof(id)}
            """;

        logger.LogInformation("Removing product by ID {Id}", id);
        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            if (rowsAffected > 0)
            {
                logger.LogInformation("Product removed successfully with ID {Id}", id);
                return true;
            }
            else
            {
                logger.LogInformation("No product found with ID {Id} to remove", id);
                return false;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while removing product by ID {Id}", id);
            throw new DatabaseException(e);
        }
    }

    public async Task<bool> AddAsync(ProductEntity product)
    {
        var sql =
            $"""
            INSERT INTO {ProductSchema.Table}
                ({ProductSchema.Columns.Id},
                 {ProductSchema.Columns.BrandId},
                 {ProductSchema.Columns.CategoryId},
                 {ProductSchema.Columns.Name},
                 {ProductSchema.Columns.Slug},
                 {ProductSchema.Columns.Description},
                 {ProductSchema.Columns.Price},
                 {ProductSchema.Columns.Discount},
                 {ProductSchema.Columns.SKU},
                 {ProductSchema.Columns.Stock},
                 {ProductSchema.Columns.Availability})
            VALUES 
                (@{nameof(product.Id)},
                 @{nameof(product.BrandId)},
                 @{nameof(product.CategoryId)},
                 @{nameof(product.Name)},
                 @{nameof(product.Slug)},
                 @{nameof(product.Description)},
                 @{nameof(product.Price)},
                 @{nameof(product.Discount)},
                 @{nameof(product.SKU)},
                 @{nameof(product.Stock)},
                 @{nameof(product.Availability)})
            """;

        logger.LogInformation("Adding new product with ID {Id}", product.Id);
        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, product);

            if (rowsAffected > 0)
            {
                logger.LogInformation("Product added successfully with ID {Id}", product.Id);
                return true;
            }
            else
            {
                logger.LogInformation("Product could not be added with ID {Id}", product.Id);
                return false;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while adding new product with ID {Id}", product.Id);
            throw new DatabaseException(e);
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var sql =
            $"""
            SELECT EXISTS (SELECT 1
                           FROM {ProductSchema.Table}
                           WHERE {ProductSchema.Columns.Id} = @{nameof(id)})
            """;

        logger.LogInformation("Checking if product exists with ID {Id}", id);
        try
        {
            var exists = await connection.ExecuteScalarAsync<bool>(sql, new { Id = id });

            if (exists)
            {
                logger.LogInformation("Product exists with ID {Id}", id);
                return exists;
            }
            else
            {
                logger.LogInformation("Product does not exist with ID {Id}", id);
                return exists;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while checking if product exists with ID {Id}", id);
            throw new DatabaseException(e);
        }
    }
}