using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Exceptions;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Catalog.Infrastructure.Database.Schemas;
using Dapper;

namespace Catalog.Infrastructure.Database.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IDbConnection connection;

    public ProductRepository(DapperDbContext context)
    {
        connection = context.Connection;
    }

    public IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        connection.Open();
        return connection.BeginTransaction(isolationLevel);
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

            return productDictionary.Values.ToList();
        }
        catch (Exception e)
        {
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

            return productDictionary.Values.FirstOrDefault();
        }
        catch (Exception e)
        {
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
                {ProductSchema.Columns.Description} = @{nameof(product.Description)},
                {ProductSchema.Columns.Price} = @{nameof(product.Price)},
                {ProductSchema.Columns.Discount} = @{nameof(product.Discount)},
                {ProductSchema.Columns.SKU} = @{nameof(product.SKU)},
                {ProductSchema.Columns.Stock} = @{nameof(product.Stock)},
                {ProductSchema.Columns.Availability} = @{nameof(product.Availability)}
            WHERE {ProductSchema.Columns.Id} = @{nameof(product.Id)}
            """;

        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, product);

            return rowsAffected > 0;
        }
        catch (Exception e)
        {
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

        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

            return rowsAffected > 0;
        }
        catch (Exception e)
        {
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
                 @{nameof(product.Description)},
                 @{nameof(product.Price)},
                 @{nameof(product.Discount)},
                 @{nameof(product.SKU)},
                 @{nameof(product.Stock)},
                 @{nameof(product.Availability)})
            """;

        try
        {
            var rowsAffected = await connection.ExecuteAsync(sql, product);

            return rowsAffected > 0;
        }
        catch (Exception e)
        {
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

        try
        {
            var exists = await connection.ExecuteScalarAsync<bool>(sql, new { Id = id });

            return exists;
        }
        catch (Exception e)
        {
            throw new DatabaseException(e);
        }
    }
}