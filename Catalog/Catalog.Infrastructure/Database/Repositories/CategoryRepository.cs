using System.Data;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Database.Repositories.Abstractions;
using Dapper;

namespace Catalog.Infrastructure.Database.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IDbConnection connection;

    public CategoryRepository(DbContext context)
    {
        connection = context.Connection;
    }

    public async Task<List<CategoryEntity>> GetAllAsync()
    {
        const string sql =
            """
            SELECT * FROM "Categories"
            """;

        var res = await connection.QueryAsync<CategoryEntity>(sql);

        return res.ToList();
    }

    public async Task<CategoryEntity?> GetByIdAsync(Guid id)
    {
        const string sql =
            """
            SELECT * FROM "Categories" WHERE "Id" = @Id
            """;

        var res = await connection.QuerySingleOrDefaultAsync<CategoryEntity>(sql, new { Id = id });

        return res;
    }

    public async Task<bool> UpdateAsync(CategoryEntity category)
    {
        const string sql =
            """
            UPDATE "Categories" SET
                "ParentCategoryId" = @ParentCategoryId,
                "Name" = @Name,
                "Description" = @Description,
                "ImageUrl" = @ImageUrl,
                "DisplayOrder" = @DisplayOrder
            WHERE "Id" = @Id
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, category);

        return rowsAffected > 0;
    }

    public async Task<bool> RemoveByIdAsync(Guid id)
    {
        const string sql =
            """
            DELETE FROM "Categories" WHERE "Id" = @Id
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });

        return rowsAffected > 0;
    }

    public async Task<bool> AddAsync(CategoryEntity category)
    {
        const string sql =
            """
            INSERT INTO "Categories" ("Id", "ParentCategoryId", "Name", "Description", "ImageUrl", "DisplayOrder")
            VALUES (@Id, @ParentCategoryId, @Name, @Description, @ImageUrl, @DisplayOrder)
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, category);

        return rowsAffected > 0;
    }
}