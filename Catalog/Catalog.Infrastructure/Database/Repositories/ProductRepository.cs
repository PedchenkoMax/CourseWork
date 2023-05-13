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
            SELECT * FROM "Products"
            """;

        var res = await connection.QueryAsync<ProductEntity>(sql);

        return res.ToList();
    }

    public async Task<ProductEntity?> GetByIdAsync(Guid id)
    {
        const string sql =
            """
            SELECT * FROM "Products" WHERE "Id" = @Id
            """;

        var res = await connection.QuerySingleOrDefaultAsync<ProductEntity>(sql, new { Id = id });

        return res;
    }

    public async Task<bool> UpdateAsync(ProductEntity product)
    {
        const string sql =
            """
            UPDATE "Products" SET
                "BrandId" = @BrandId,
                "CategoryId" = @CategoryId,
                "Name" = @Name,
                "Description" = @Description,
                "Price" = @Price,
                "Discount" = @Discount,
                "SKU" = @SKU,
                "Stock" = @Stock,
                "Availability" = @Availability
            WHERE "Id" = @Id
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
            DELETE FROM "Products" WHERE "Id" = @Id
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

        return rowsAffected > 0;
    }

    public async Task<bool> AddAsync(ProductEntity product)
    {
        const string sql =
            """
            INSERT INTO "Products" ("Id", "BrandId", "CategoryId", "Name", "Description", "Price", "Discount", "SKU", "Stock", "Availability")
            VALUES (@Id, @BrandId, @CategoryId, @Name, @Description, @Price, @Discount, @SKU, @Stock, @Availability)
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, product);

        return rowsAffected > 0;
    }
}