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
            SELECT * FROM products
            """;

        var res = await connection.QueryAsync<ProductEntity>(sql);

        return res.ToList();
    }

    public async Task<ProductEntity?> GetByIdAsync(Guid id)
    {
        const string sql =
            """
            SELECT * FROM products WHERE id = @Id
            """;

        var res = await connection.QuerySingleOrDefaultAsync<ProductEntity>(sql, new { Id = id });

        return res;
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