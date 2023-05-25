using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Dapper;

namespace Catalog.Infrastructure.Database.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IDbConnection connection;

    public ProductRepository(DbContext context)
    {
        connection = context.Connection;
    }

    public async Task<List<ProductEntity>> GetAllAsync()
    {
        const string sql =
            """
            SELECT p.*, b.*, c.*, i.*
            FROM products p
            LEFT JOIN brands b ON p.brand_id = b.Id
            LEFT JOIN categories c ON p.category_id = c.Id
            LEFT JOIN product_images i ON p.Id = i.product_id
            """;

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
            splitOn: "Id,Id,Id");

        return productDictionary.Values.ToList();
    }

    public async Task<ProductEntity?> GetByIdAsync(Guid id)
    {
        const string sql =
            """
            SELECT p.*, b.*, c.*, i.*
            FROM products p
            LEFT JOIN brands b ON p.brand_id = b.Id
            LEFT JOIN categories c ON p.category_id = c.Id
            LEFT JOIN product_images i ON p.Id = i.product_id
            WHERE p.id = @Id
            """;

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
            splitOn: "Id,Id,Id");

        return productDictionary.Values.FirstOrDefault();
    }

    public async Task<bool> UpdateAsync(ProductEntity product)
    {
        const string sql =
            """
            UPDATE products SET
                brand_Id = @BrandId,
                category_Id = @CategoryId,
                name = @Name,
                description = @Description,
                price = @Price,
                discount = @Discount,
                sku = @SKU,
                stock = @Stock,
                availability = @Availability
            WHERE id = @Id
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, product);

        return rowsAffected > 0;
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        // TODO: should i delete productImages in cascade?
        // do it in sql query or db configs

        const string sql =
            """
            DELETE FROM products WHERE id = @Id
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

        return rowsAffected > 0;
    }

    public async Task<bool> AddAsync(ProductEntity product)
    {
        const string sql =
            """
            INSERT INTO products (id, brand_id, category_id, name, description, price, discount, sku, stock, availability)
            VALUES (@Id, @BrandId, @CategoryId, @Name, @Description, @Price, @Discount, @SKU, @Stock, @Availability)
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, product);

        return rowsAffected > 0;
    }
    
    public async Task<bool> ExistsAsync(Guid id)
    {
        const string sql =
            """
            SELECT EXISTS (SELECT 1 FROM products WHERE id = @Id)
            """;

        var exists = await connection.ExecuteScalarAsync<bool>(sql, new { Id = id });

        return exists;
    }
}