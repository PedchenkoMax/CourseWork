using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Dapper;

namespace Catalog.Infrastructure.Database.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly IDbConnection connection;

    public ProductImageRepository(DbContext context)
    {
        connection = context.Connection;
    }

    public async Task<ProductImageEntity?> GetByProductIdAsync(Guid id)
    {
        const string sql =
            """
            SELECT * FROM product_images WHERE product_id = @Id
            """;

        var res = await connection.QuerySingleOrDefaultAsync<ProductImageEntity>(sql, new { Id = id });

        return res;
    }

    public async Task<ProductImageEntity?> GetByIdAsync(Guid id)
    {
        const string sql =
            """
            SELECT * FROM product_images WHERE id = @Id
            """;

        var res = await connection.QuerySingleOrDefaultAsync<ProductImageEntity>(sql, new { Id = id });

        return res;
    }

    public async Task<bool> UpdateAsync(ProductImageEntity productImage)
    {
        const string sql =
            """
            UPDATE product_images SET
                product_id = @ProductId,
                image_url = @ImageUrl,
                display_order = @DisplayOrder
            WHERE id = @Id
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, productImage);

        return rowsAffected > 0;
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        const string sql =
            """
            DELETE FROM product_images WHERE id = @Id
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

        return rowsAffected > 0;
    }

    public async Task<bool> AddAsync(ProductImageEntity productImage)
    {
        const string sql =
            """
            INSERT INTO product_images (id, product_id, image_url, display_order)
            VALUES (@Id, @ProductId, @ImageUrl, @DisplayOrder)
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, productImage);

        return rowsAffected > 0;
    }
}